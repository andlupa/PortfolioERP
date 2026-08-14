using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Common;
using PortfolioERP.Application.Common.Exceptions;
using PortfolioERP.Application.Features.Products;
using PortfolioERP.Domain.Entities;
using PortfolioERP.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace PortfolioERP.Infrastructure.Services;

public sealed class ProductService : IProductService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext dbContext, ILogger<ProductService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductResponse>> GetAllAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        ValidateQueryParameters(parameters);

        var query = _dbContext.Products
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();

            query = query.Where(product =>
                EF.Functions.Like(product.Code, $"%{search}%") ||
                EF.Functions.Like(product.Name, $"%{search}%") ||
                (
                    product.Description != null &&
                    EF.Functions.Like(
                        product.Description,
                        $"%{search}%")
                ));
        }

        if (parameters.CategoryId.HasValue)
        {
            query = query.Where(product =>
                product.CategoryId == parameters.CategoryId.Value);
        }

        if (parameters.IsActive.HasValue)
        {
            query = query.Where(product =>
                product.IsActive == parameters.IsActive.Value);
        }

        if (parameters.MinPrice.HasValue)
        {
            query = query.Where(product =>
                product.Price >= parameters.MinPrice.Value);
        }

        if (parameters.MaxPrice.HasValue)
        {
            query = query.Where(product =>
                product.Price <= parameters.MaxPrice.Value);
        }

        query = ApplySorting(
            query,
            parameters.SortBy,
            parameters.Descending);

        var totalItems = await query.CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)parameters.PageSize);

        var products = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(product => new ProductResponse(
                product.Id,
                product.Code,
                product.Name,
                product.Description,
                product.Price,
                product.VatPercentage,
                product.StockQuantity,
                product.IsActive,
                product.CreatedAtUtc,
                product.CategoryId,
                product.Category.Name))
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProductResponse>(
            products,
            parameters.PageNumber,
            parameters.PageSize,
            totalItems,
            totalPages);
    }

    public async Task<ProductResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductResponse(
                product.Id,
                product.Code,
                product.Name,
                product.Description,
                product.Price,
                product.VatPercentage,
                product.StockQuantity,
                product.IsActive,
                product.CreatedAtUtc,
                product.CategoryId,
                product.Category.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var categoryExists = await _dbContext.Categories
            .AnyAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            throw new ValidationException(
                "The selected category does not exist.");
        }

        var codeExists = await _dbContext.Products
            .AnyAsync(
                product => product.Code == normalizedCode,
                cancellationToken);

        if (codeExists)
        {
            throw new ConflictException(
                "A product with the same code already exists.");
        }

        var product = new Product
        {
            Code = normalizedCode,
            Name = request.Name.Trim(),
            Description = NormalizeOptional(request.Description),
            Price = request.Price,
            VatPercentage = request.VatPercentage,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId
        };

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var categoryName = await _dbContext.Categories
            .Where(category => category.Id == product.CategoryId)
            .Select(category => category.Name)
            .FirstAsync(cancellationToken);

        _logger.LogInformation("Creating product with code {ProductCode} and name {ProductName}",
            request.Code,
            request.Name);

        return MapToResponse(product, categoryName);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);

        if (product is null)
        {
            _logger.LogWarning("Product update failed because product {ProductId} was not found", id);
            
            return false;
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var categoryExists = await _dbContext.Categories
            .AnyAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            throw new ValidationException(
                "The selected category does not exist.");
        }

        var duplicateCode = await _dbContext.Products
            .AnyAsync(
                other =>
                    other.Id != id &&
                    other.Code == normalizedCode,
                cancellationToken);

        if (duplicateCode)
        {
            throw new ConflictException(
                "Another product with the same code already exists.");
        }

        product.Code = normalizedCode;
        product.Name = request.Name.Trim();
        product.Description = NormalizeOptional(request.Description);
        product.Price = request.Price;
        product.VatPercentage = request.VatPercentage;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updating product with code {ProductCode} and name {ProductName}",
            request.Code,
            request.Name);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.IsActive = false;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void ValidateQueryParameters(
        ProductQueryParameters parameters)
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

        if (parameters.MinPrice is < 0)
        {
            throw new ValidationException(
                "MinPrice cannot be negative.");
        }

        if (parameters.MaxPrice is < 0)
        {
            throw new ValidationException(
                "MaxPrice cannot be negative.");
        }

        if (parameters.MinPrice.HasValue &&
            parameters.MaxPrice.HasValue &&
            parameters.MinPrice > parameters.MaxPrice)
        {
            throw new ValidationException(
                "MinPrice cannot be greater than MaxPrice.");
        }
    }

    private static ProductResponse MapToResponse(
        Product product,
        string categoryName)
    {
        return new ProductResponse(
            product.Id,
            product.Code,
            product.Name,
            product.Description,
            product.Price,
            product.VatPercentage,
            product.StockQuantity,
            product.IsActive,
            product.CreatedAtUtc,
            product.CategoryId,
            categoryName);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        string? sortBy,
        bool descending)
    {
        var normalizedSortBy = sortBy?
            .Trim()
            .ToLowerInvariant();

        return (normalizedSortBy, descending) switch
        {
            ("code", false) =>
                query.OrderBy(product => product.Code)
                    .ThenBy(product => product.Id),

            ("code", true) =>
                query.OrderByDescending(product => product.Code)
                    .ThenByDescending(product => product.Id),

            ("price", false) =>
                query.OrderBy(product => product.Price)
                    .ThenBy(product => product.Id),

            ("price", true) =>
                query.OrderByDescending(product => product.Price)
                    .ThenByDescending(product => product.Id),

            ("stockquantity", false) =>
                query.OrderBy(product => product.StockQuantity)
                    .ThenBy(product => product.Id),

            ("stockquantity", true) =>
                query.OrderByDescending(product => product.StockQuantity)
                    .ThenByDescending(product => product.Id),

            ("category", false) =>
                query.OrderBy(product => product.Category.Name)
                    .ThenBy(product => product.Id),

            ("category", true) =>
                query.OrderByDescending(product => product.Category.Name)
                    .ThenByDescending(product => product.Id),

            ("createdatutc", false) =>
                query.OrderBy(product => product.CreatedAtUtc)
                    .ThenBy(product => product.Id),

            ("createdatutc", true) =>
                query.OrderByDescending(product => product.CreatedAtUtc)
                    .ThenByDescending(product => product.Id),

            (_, false) =>
                query.OrderBy(product => product.Name)
                    .ThenBy(product => product.Id),

            (_, true) =>
                query.OrderByDescending(product => product.Name)
                    .ThenByDescending(product => product.Id)
        };
    }
}