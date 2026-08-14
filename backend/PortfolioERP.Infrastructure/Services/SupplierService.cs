using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Common;
using PortfolioERP.Application.Features.Suppliers;
using PortfolioERP.Domain.Entities;
using PortfolioERP.Infrastructure.Persistence;

namespace PortfolioERP.Infrastructure.Services;

public sealed class SupplierService : ISupplierService
{
    private readonly AppDbContext _dbContext;

    public SupplierService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<SupplierResponse>> GetAllAsync(
        SupplierQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Suppliers
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim().ToLower();

            query = query.Where(supplier =>
                supplier.SupplierCode.ToLower().Contains(search) ||
                supplier.CompanyName.ToLower().Contains(search) ||
                supplier.Email.ToLower().Contains(search) ||
                (supplier.VatNumber != null &&
                 supplier.VatNumber.ToLower().Contains(search)) ||
                (supplier.TaxCode != null &&
                 supplier.TaxCode.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(parameters.City))
        {
            var city = parameters.City.Trim().ToLower();

            query = query.Where(supplier =>
                supplier.City != null &&
                supplier.City.ToLower().Contains(city));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Country))
        {
            var country = parameters.Country.Trim().ToLower();

            query = query.Where(supplier =>
                supplier.Country.ToLower().Contains(country));
        }

        if (parameters.IsActive.HasValue)
        {
            query = query.Where(supplier =>
                supplier.IsActive == parameters.IsActive.Value);
        }

        query = parameters.SortBy.ToLowerInvariant() switch
        {
            "suppliercode" => parameters.Descending
                ? query.OrderByDescending(x => x.SupplierCode)
                : query.OrderBy(x => x.SupplierCode),

            "email" => parameters.Descending
                ? query.OrderByDescending(x => x.Email)
                : query.OrderBy(x => x.Email),

            "city" => parameters.Descending
                ? query.OrderByDescending(x => x.City)
                : query.OrderBy(x => x.City),

            "createdat" => parameters.Descending
                ? query.OrderByDescending(x => x.CreatedAtUtc)
                : query.OrderBy(x => x.CreatedAtUtc),

            _ => parameters.Descending
                ? query.OrderByDescending(x => x.CompanyName)
                : query.OrderBy(x => x.CompanyName)
        };

        var totalItems =
            await query.CountAsync(cancellationToken);

        var pageNumber =
            Math.Max(parameters.PageNumber, 1);

        var pageSize =
            Math.Clamp(parameters.PageSize, 1, 100);

        var totalPages = (int)Math.Ceiling(
             totalItems / (double)pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(supplier =>
                new SupplierResponse(
                    supplier.Id,
                    supplier.SupplierCode,
                    supplier.CompanyName,
                    supplier.ContactName,
                    supplier.VatNumber,
                    supplier.TaxCode,
                    supplier.Email,
                    supplier.Phone,
                    supplier.Address,
                    supplier.City,
                    supplier.Province,
                    supplier.PostalCode,
                    supplier.Country,
                    supplier.IsActive,
                    supplier.CreatedAtUtc,
                    supplier.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResponse<SupplierResponse>(
            items,
            totalItems,
            pageNumber,
            pageSize,
            totalPages);
    }

    public async Task<SupplierResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Suppliers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(supplier =>
                new SupplierResponse(
                    supplier.Id,
                    supplier.SupplierCode,
                    supplier.CompanyName,
                    supplier.ContactName,
                    supplier.VatNumber,
                    supplier.TaxCode,
                    supplier.Email,
                    supplier.Phone,
                    supplier.Address,
                    supplier.City,
                    supplier.Province,
                    supplier.PostalCode,
                    supplier.Country,
                    supplier.IsActive,
                    supplier.CreatedAtUtc,
                    supplier.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SupplierResponse> CreateAsync(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedCode =
            request.SupplierCode.Trim().ToLowerInvariant();

        var codeExists = await _dbContext.Suppliers
            .AnyAsync(
                supplier =>
                    supplier.SupplierCode.ToLower() ==
                    normalizedCode,
                cancellationToken);

        if (codeExists)
        {
            throw new InvalidOperationException(
                "Supplier code already exists.");
        }

        var supplier = new Supplier
        {
            SupplierCode = request.SupplierCode.Trim(),
            CompanyName = request.CompanyName.Trim(),
            ContactName = Normalize(request.ContactName),
            VatNumber = Normalize(request.VatNumber),
            TaxCode = Normalize(request.TaxCode),
            Email = request.Email.Trim(),
            Phone = Normalize(request.Phone),
            Address = Normalize(request.Address),
            City = Normalize(request.City),
            Province = Normalize(request.Province),
            PostalCode = Normalize(request.PostalCode),
            Country = request.Country.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Suppliers.Add(supplier);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return MapSupplier(supplier);
    }

    public async Task<SupplierResponse?> UpdateAsync(
        int id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (supplier is null)
        {
            return null;
        }

        var normalizedCode =
            request.SupplierCode.Trim().ToLowerInvariant();

        var duplicateCode = await _dbContext.Suppliers
            .AnyAsync(
                x =>
                    x.Id != id &&
                    x.SupplierCode.ToLower() ==
                    normalizedCode,
                cancellationToken);

        if (duplicateCode)
        {
            throw new InvalidOperationException(
                "Supplier code already exists.");
        }

        supplier.SupplierCode =
            request.SupplierCode.Trim();

        supplier.CompanyName =
            request.CompanyName.Trim();

        supplier.ContactName =
            Normalize(request.ContactName);

        supplier.VatNumber =
            Normalize(request.VatNumber);

        supplier.TaxCode =
            Normalize(request.TaxCode);

        supplier.Email =
            request.Email.Trim();

        supplier.Phone =
            Normalize(request.Phone);

        supplier.Address =
            Normalize(request.Address);

        supplier.City =
            Normalize(request.City);

        supplier.Province =
            Normalize(request.Province);

        supplier.PostalCode =
            Normalize(request.PostalCode);

        supplier.Country =
            request.Country.Trim();

        supplier.IsActive =
            request.IsActive;

        supplier.UpdatedAtUtc =
            DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return MapSupplier(supplier);
    }

    public async Task<bool> DeactivateAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (supplier is null)
        {
            return false;
        }

        supplier.IsActive = false;
        supplier.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static SupplierResponse MapSupplier(
        Supplier supplier)
    {
        return new SupplierResponse(
            supplier.Id,
            supplier.SupplierCode,
            supplier.CompanyName,
            supplier.ContactName,
            supplier.VatNumber,
            supplier.TaxCode,
            supplier.Email,
            supplier.Phone,
            supplier.Address,
            supplier.City,
            supplier.Province,
            supplier.PostalCode,
            supplier.Country,
            supplier.IsActive,
            supplier.CreatedAtUtc,
            supplier.UpdatedAtUtc);
    }

    private static string? Normalize(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}