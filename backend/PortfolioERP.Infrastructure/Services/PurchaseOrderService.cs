using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Features.PurchaseOrders;
using PortfolioERP.Domain.Entities;
using PortfolioERP.Domain.Enums;
using PortfolioERP.Infrastructure.Persistence;

namespace PortfolioERP.Infrastructure.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly AppDbContext _context;

    public PurchaseOrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PurchaseOrderListResponse>>
        GetAllAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.Id)
            .Select(o => new PurchaseOrderListResponse(
                o.Id,
                o.OrderNumber,
                o.SupplierId,
                o.Supplier.CompanyName,
                o.OrderDate,
                o.Status,
                o.TotalAmount))
            .ToListAsync(cancellationToken);
    }

    public async Task<PurchaseOrderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new PurchaseOrderResponse(
                o.Id,
                o.OrderNumber,
                o.SupplierId,
                o.Supplier.SupplierCode,
                o.Supplier.CompanyName,
                o.OrderDate,
                o.Status,
                o.NetAmount,
                o.VatAmount,
                o.TotalAmount,
                o.Notes,
                o.CreatedAtUtc,
                o.UpdatedAtUtc,
                o.Lines
                    .OrderBy(l => l.Id)
                    .Select(l =>
                        new PurchaseOrderLineResponse(
                            l.Id,
                            l.ProductId,
                            l.Product.Code,
                            l.Product.Name,
                            l.Quantity,
                            l.UnitPrice,
                            l.DiscountPercentage,
                            l.DiscountAmount,
                            l.NetAmount,
                            l.VatPercentage,
                            l.VatAmount,
                            l.TotalAmount))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PurchaseOrderResponse> CreateAsync(
        CreatePurchaseOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(
                s =>
                    s.Id == request.SupplierId &&
                    s.IsActive,
                cancellationToken);

        if (supplier is null)
        {
            throw new InvalidOperationException(
                "Supplier not found or inactive.");
        }

        var productIds = request.Lines
            .Select(l => l.ProductId)
            .Distinct()
            .ToList();

        var products = await _context.Products
            .Where(p =>
                productIds.Contains(p.Id) &&
                p.IsActive)
            .ToDictionaryAsync(
                p => p.Id,
                cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new InvalidOperationException(
                "One or more products do not exist or are inactive.");
        }

        var order = new PurchaseOrder
        {
            OrderNumber = await GenerateOrderNumberAsync(
                cancellationToken),

            SupplierId = request.SupplierId,
            OrderDate = request.OrderDate,
            Status = PurchaseOrderStatus.Draft,
            Notes = request.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        foreach (var requestLine in request.Lines)
        {
            var amounts =
                PurchaseOrderCalculator.CalculateLine(
                    requestLine.Quantity,
                    requestLine.UnitPrice,
                    requestLine.DiscountPercentage,
                    requestLine.VatPercentage);

            var line = new PurchaseOrderLine
            {
                ProductId = requestLine.ProductId,
                Quantity = requestLine.Quantity,
                UnitPrice = requestLine.UnitPrice,
                DiscountPercentage =
                    requestLine.DiscountPercentage,

                DiscountAmount =
                    amounts.DiscountAmount,

                NetAmount =
                    amounts.NetAmount,

                VatPercentage =
                    requestLine.VatPercentage,

                VatAmount =
                    amounts.VatAmount,

                TotalAmount =
                    amounts.TotalAmount
            };

            order.Lines.Add(line);
        }

        order.NetAmount =
            order.Lines.Sum(l => l.NetAmount);

        order.VatAmount =
            order.Lines.Sum(l => l.VatAmount);

        order.TotalAmount =
            order.Lines.Sum(l => l.TotalAmount);

        _context.PurchaseOrders.Add(order);

        await _context.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
                   order.Id,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   "Unable to load the created purchase order.");
    }

    private async Task<string> GenerateOrderNumberAsync(
        CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;

        var prefix = $"PO-{year}-";

        var lastOrderNumber =
            await _context.PurchaseOrders
                .Where(o =>
                    o.OrderNumber.StartsWith(prefix))
                .OrderByDescending(o => o.OrderNumber)
                .Select(o => o.OrderNumber)
                .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = 1;

        if (
            lastOrderNumber is not null &&
            int.TryParse(
                lastOrderNumber[prefix.Length..],
                out var lastNumber))
        {
            nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D5}";
    }

    public async Task<PurchaseOrderResponse> MarkAsOrderedAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var order = await _context.PurchaseOrders
            .FirstOrDefaultAsync(
                o => o.Id == id,
                cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }

        if (order.Status != PurchaseOrderStatus.Draft)
        {
            throw new InvalidOperationException(
                "Only draft purchase orders can be marked as ordered.");
        }

        order.Status = PurchaseOrderStatus.Ordered;
        order.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException(
                "Unable to load purchase order.");
    }

    public async Task<PurchaseOrderResponse> ReceiveAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var order = await _context.PurchaseOrders
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(
                    o => o.Id == id,
                    cancellationToken);

            if (order is null)
            {
                throw new KeyNotFoundException(
                    "Purchase order not found.");
            }

            if (order.Status != PurchaseOrderStatus.Ordered)
            {
                throw new InvalidOperationException(
                    "Only ordered purchase orders can be received.");
            }

            var productIds = order.Lines
                .Select(l => l.ProductId)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(
                    p => p.Id,
                    cancellationToken);

            if (products.Count != productIds.Count)
            {
                throw new InvalidOperationException(
                    "One or more products were not found.");
            }

            foreach (var line in order.Lines)
            {
                var product = products[line.ProductId];

                product.StockQuantity += line.Quantity;
            }

            order.Status = PurchaseOrderStatus.Received;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }

        return await GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException(
                "Unable to load purchase order.");
    }

    public async Task<PurchaseOrderResponse> CancelAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var order = await _context.PurchaseOrders
            .FirstOrDefaultAsync(
                o => o.Id == id,
                cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }

        if (order.Status == PurchaseOrderStatus.Received)
        {
            throw new InvalidOperationException(
                "A received purchase order cannot be cancelled.");
        }

        if (order.Status == PurchaseOrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Purchase order is already cancelled.");
        }

        order.Status = PurchaseOrderStatus.Cancelled;
        order.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException(
                "Unable to load purchase order.");
    }
}