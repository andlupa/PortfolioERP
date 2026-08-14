using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Common;
using PortfolioERP.Application.Common.Exceptions;
using PortfolioERP.Application.Features.Orders;
using PortfolioERP.Domain.Entities;
using PortfolioERP.Domain.Enums;
using PortfolioERP.Domain.Services.Orders;
using PortfolioERP.Infrastructure.Persistence;

namespace PortfolioERP.Infrastructure.Services;

public sealed class OrderService : IOrderService
{
    private readonly AppDbContext _dbContext;
    private readonly IOrderCalculator _orderCalculator;

    public OrderService(
        AppDbContext dbContext,
        IOrderCalculator orderCalculator)
    {
        _dbContext = dbContext;
        _orderCalculator = orderCalculator;
    }

    public async Task<PagedResponse<OrderListItemResponse>> GetAllAsync(
        OrderQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        ValidateQueryParameters(parameters);

        var query = _dbContext.SalesOrders
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();

            query = query.Where(order =>
                EF.Functions.Like(order.OrderNumber, $"%{search}%") ||
                EF.Functions.Like(
                    order.Customer.CompanyName,
                    $"%{search}%"));
        }

        if (parameters.CustomerId.HasValue)
        {
            query = query.Where(order =>
                order.CustomerId == parameters.CustomerId.Value);
        }

        if (parameters.Status.HasValue)
        {
            query = query.Where(order =>
                order.Status == parameters.Status.Value);
        }

        if (parameters.DateFrom.HasValue)
        {
            query = query.Where(order =>
                order.OrderDate >= parameters.DateFrom.Value);
        }

        if (parameters.DateTo.HasValue)
        {
            query = query.Where(order =>
                order.OrderDate <= parameters.DateTo.Value);
        }

        if (parameters.MinTotal.HasValue)
        {
            query = query.Where(order =>
                order.TotalAmount >= parameters.MinTotal.Value);
        }

        if (parameters.MaxTotal.HasValue)
        {
            query = query.Where(order =>
                order.TotalAmount <= parameters.MaxTotal.Value);
        }

        query = ApplySorting(
            query,
            parameters.SortBy,
            parameters.Descending);

        var totalItems = await query.CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)parameters.PageSize);

        var orders = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(order => new OrderListItemResponse(
                order.Id,
                order.OrderNumber,
                order.OrderDate,
                order.Status,
                order.CustomerId,
                order.Customer.CompanyName,
                order.Lines.Count,
                order.TotalAmount))
            .ToListAsync(cancellationToken);

        return new PagedResponse<OrderListItemResponse>(
            orders,
            parameters.PageNumber,
            parameters.PageSize,
            totalItems,
            totalPages);
    }

    public async Task<OrderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.SalesOrders
            .AsNoTracking()
            .Where(order => order.Id == id)
            .Select(order => new OrderResponse(
                order.Id,
                order.OrderNumber,
                order.OrderDate,
                order.Status,
                order.CustomerId,
                order.Customer.CompanyName,
                order.Notes,
                order.Subtotal,
                order.DiscountAmount,
                order.TaxAmount,
                order.TotalAmount,
                order.CreatedAtUtc,
                order.UpdatedAtUtc,
                order.Lines
                    .OrderBy(line => line.Id)
                    .Select(line => new OrderLineResponse(
                        line.Id,
                        line.ProductId,
                        line.Product.Code,
                        line.Product.Name,
                        line.Quantity,
                        line.UnitPrice,
                        line.DiscountPercentage,
                        line.DiscountAmount,
                        line.NetAmount,
                        line.VatPercentage,
                        line.VatAmount,
                        line.TotalAmount))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                customer =>
                    customer.Id == request.CustomerId &&
                    customer.IsActive,
                cancellationToken);

        if (customer is null)
        {
            throw new ValidationException(
                "The selected customer does not exist or is inactive.");
        }

        var duplicateProductIds = request.Lines
            .GroupBy(line => line.ProductId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateProductIds.Count > 0)
        {
            throw new ValidationException(
                "Each product can appear only once in an order.");
        }

        var productIds = request.Lines
            .Select(line => line.ProductId)
            .Distinct()
            .ToList();

        var products = await _dbContext.Products
            .Where(product =>
                productIds.Contains(product.Id) &&
                product.IsActive)
            .ToDictionaryAsync(
                product => product.Id,
                cancellationToken);

        var missingProductIds = productIds
            .Where(productId => !products.ContainsKey(productId))
            .ToList();

        if (missingProductIds.Count > 0)
        {
            throw new ValidationException(
                $"Products not found or inactive: {string.Join(", ", missingProductIds)}.");
        }

        var calculationInputs =
            new List<OrderLineCalculationInput>();

        foreach (var requestedLine in request.Lines)
        {
            var product = products[requestedLine.ProductId];

            if (requestedLine.Quantity > product.StockQuantity)
            {
                throw new ConflictException(
                    $"Insufficient stock for product {product.Code}. " +
                    $"Available: {product.StockQuantity}; " +
                    $"requested: {requestedLine.Quantity}.");
            }

            calculationInputs.Add(
                new OrderLineCalculationInput(
                    product.Id,
                    requestedLine.Quantity,
                    product.Price,
                    requestedLine.DiscountPercentage,
                    product.VatPercentage));
        }

        var calculation = _orderCalculator.Calculate(
            calculationInputs);

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var order = new SalesOrder
            {
                OrderNumber = await GenerateOrderNumberAsync(
                    cancellationToken),
                CustomerId = customer.Id,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Draft,
                Notes = NormalizeOptional(request.Notes),
                Subtotal = calculation.Subtotal,
                DiscountAmount = calculation.DiscountAmount,
                TaxAmount = calculation.TaxAmount,
                TotalAmount = calculation.TotalAmount
            };

            foreach (var calculatedLine in calculation.Lines)
            {
                var product = products[calculatedLine.ProductId];

                order.Lines.Add(new SalesOrderLine
                {
                    ProductId = calculatedLine.ProductId,
                    Quantity = calculatedLine.Quantity,
                    UnitPrice = calculatedLine.UnitPrice,
                    DiscountPercentage =
                        calculatedLine.DiscountPercentage,
                    DiscountAmount =
                        calculatedLine.DiscountAmount,
                    NetAmount =
                        calculatedLine.NetAmount,
                    VatPercentage =
                        calculatedLine.VatPercentage,
                    VatAmount =
                        calculatedLine.VatAmount,
                    TotalAmount =
                        calculatedLine.TotalAmount
                });

                product.StockQuantity -= calculatedLine.Quantity;
            
            }

            _dbContext.SalesOrders.Add(order);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await GetByIdAsync(order.Id, cancellationToken)
                ?? throw new InvalidOperationException(
                    "The created order could not be loaded.");
            
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> ConfirmAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _dbContext.SalesOrders
            .FirstOrDefaultAsync(
                order => order.Id == id,
                cancellationToken);

        if (order is null)
        {
            return false;
        }

        if (order.Status != OrderStatus.Draft)
        {
            throw new ConflictException(
                "Only draft orders can be confirmed.");
        }

        order.Status = OrderStatus.Confirmed;
        order.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> CancelAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _dbContext.SalesOrders
            .Include(order => order.Lines)
            .FirstOrDefaultAsync(
                order => order.Id == id,
                cancellationToken);

        if (order is null)
        {
            return false;
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return true;
        }

        if (order.Status is OrderStatus.Shipped or
            OrderStatus.Completed)
        {
            throw new ConflictException(
                "Shipped or completed orders cannot be cancelled.");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var productIds = order.Lines
                .Select(line => line.ProductId)
                .Distinct()
                .ToList();

            var products = await _dbContext.Products
                .Where(product => productIds.Contains(product.Id))
                .ToDictionaryAsync(
                    product => product.Id,
                    cancellationToken);

            foreach (var line in order.Lines)
            {
                if (products.TryGetValue(line.ProductId, out var product))
                {
                    product.StockQuantity += line.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<string> GenerateOrderNumberAsync(
        CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;

        var lastOrderNumber = await _dbContext.SalesOrders
            .Where(order =>
                order.OrderNumber.StartsWith($"SO-{year}-"))
            .OrderByDescending(order => order.Id)
            .Select(order => order.OrderNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = 1;

        if (!string.IsNullOrWhiteSpace(lastOrderNumber))
        {
            var lastPart = lastOrderNumber
                .Split('-')
                .LastOrDefault();

            if (int.TryParse(lastPart, out var lastSequence))
            {
                nextSequence = lastSequence + 1;
            }
        }

        return $"SO-{year}-{nextSequence:00000}";
    }

    private static void ValidateQueryParameters(
        OrderQueryParameters parameters)
    {
        if (parameters.PageNumber < 1)
        {
            throw new ValidationException(
                "PageNumber must be greater than zero.");
        }

        if (parameters.PageSize < 1 || parameters.PageSize > 100)
        {
            throw new ValidationException(
                "PageSize must be between 1 and 100.");
        }

        if (parameters.MinTotal is < 0 ||
            parameters.MaxTotal is < 0)
        {
            throw new ValidationException(
                "Order totals cannot be negative.");
        }

        if (parameters.MinTotal.HasValue &&
            parameters.MaxTotal.HasValue &&
            parameters.MinTotal > parameters.MaxTotal)
        {
            throw new ValidationException(
                "MinTotal cannot be greater than MaxTotal.");
        }

        if (parameters.DateFrom.HasValue &&
            parameters.DateTo.HasValue &&
            parameters.DateFrom > parameters.DateTo)
        {
            throw new ValidationException(
                "DateFrom cannot be later than DateTo.");
        }
    }

    private static IQueryable<SalesOrder> ApplySorting(
        IQueryable<SalesOrder> query,
        string? sortBy,
        bool descending)
    {
        var normalizedSortBy = sortBy?
            .Trim()
            .ToLowerInvariant();

        return (normalizedSortBy, descending) switch
        {
            ("ordernumber", false) =>
                query.OrderBy(order => order.OrderNumber)
                    .ThenBy(order => order.Id),

            ("ordernumber", true) =>
                query.OrderByDescending(order => order.OrderNumber)
                    .ThenByDescending(order => order.Id),

            ("customer", false) =>
                query.OrderBy(order => order.Customer.CompanyName)
                    .ThenBy(order => order.Id),

            ("customer", true) =>
                query.OrderByDescending(
                        order => order.Customer.CompanyName)
                    .ThenByDescending(order => order.Id),

            ("status", false) =>
                query.OrderBy(order => order.Status)
                    .ThenBy(order => order.Id),

            ("status", true) =>
                query.OrderByDescending(order => order.Status)
                    .ThenByDescending(order => order.Id),

            ("total", false) =>
                query.OrderBy(order => order.TotalAmount)
                    .ThenBy(order => order.Id),

            ("total", true) =>
                query.OrderByDescending(order => order.TotalAmount)
                    .ThenByDescending(order => order.Id),

            (_, false) =>
                query.OrderBy(order => order.OrderDate)
                    .ThenBy(order => order.Id),

            (_, true) =>
                query.OrderByDescending(order => order.OrderDate)
                    .ThenByDescending(order => order.Id)
        };
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    public async Task<OrderCalculationResponse> CalculateAsync(
    CalculateOrderRequest request,
    CancellationToken cancellationToken)
    {
        if (request.Lines is null || request.Lines.Count == 0)
        {
            throw new ValidationException(
                "The order must contain at least one line.");
        }

        var duplicateProductIds = request.Lines
            .GroupBy(line => line.ProductId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateProductIds.Count > 0)
        {
            throw new ValidationException(
                "Each product can appear only once in an order.");
        }

        var productIds = request.Lines
            .Select(line => line.ProductId)
            .Distinct()
            .ToList();

        var products = await _dbContext.Products
            .AsNoTracking()
            .Where(product =>
                productIds.Contains(product.Id) &&
                product.IsActive)
            .ToDictionaryAsync(
                product => product.Id,
                cancellationToken);

        var missingProductIds = productIds
            .Where(productId => !products.ContainsKey(productId))
            .ToList();

        if (missingProductIds.Count > 0)
        {
            throw new ValidationException(
                $"Products not found or inactive: " +
                $"{string.Join(", ", missingProductIds)}.");
        }

        var calculationInputs =
            new List<OrderLineCalculationInput>();

        foreach (var requestedLine in request.Lines)
        {
            if (requestedLine.ProductId <= 0)
            {
                throw new ValidationException(
                    "ProductId must be greater than zero.");
            }

            if (requestedLine.Quantity <= 0)
            {
                throw new ValidationException(
                    "Quantity must be greater than zero.");
            }

            if (requestedLine.DiscountPercentage is < 0 or > 100)
            {
                throw new ValidationException(
                    "DiscountPercentage must be between 0 and 100.");
            }

            var product = products[requestedLine.ProductId];

            if (requestedLine.Quantity > product.StockQuantity)
            {
                throw new ConflictException(
                    $"Insufficient stock for product {product.Code}. " +
                    $"Available: {product.StockQuantity}; " +
                    $"requested: {requestedLine.Quantity}.");
            }

            calculationInputs.Add(
                new OrderLineCalculationInput(
                    product.Id,
                    requestedLine.Quantity,
                    product.Price,
                    requestedLine.DiscountPercentage,
                    product.VatPercentage));
        }

        var calculation = _orderCalculator.Calculate(
            calculationInputs);

        var lines = calculation.Lines
            .Select(line =>
            {
                var grossAmount = Math.Round(
                    line.UnitPrice * line.Quantity,
                    2,
                    MidpointRounding.AwayFromZero);

                return new OrderCalculationLineResponse(
                    line.ProductId,
                    line.UnitPrice,
                    line.Quantity,
                    grossAmount,
                    line.DiscountPercentage,
                    line.DiscountAmount,
                    line.NetAmount,
                    line.VatPercentage,
                    line.VatAmount,
                    line.TotalAmount);
            })
            .ToList();

        var netAmount = Math.Round(
            calculation.Subtotal - calculation.DiscountAmount,
            2,
            MidpointRounding.AwayFromZero);

        return new OrderCalculationResponse(
            lines,
            calculation.Subtotal,
            calculation.DiscountAmount,
            netAmount,
            calculation.TaxAmount,
            calculation.TotalAmount);
    }
}