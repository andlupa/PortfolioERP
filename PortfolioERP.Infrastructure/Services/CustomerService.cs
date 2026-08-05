using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Common;
using PortfolioERP.Application.Common.Exceptions;
using PortfolioERP.Application.Features.Customers;
using PortfolioERP.Domain.Entities;
using PortfolioERP.Infrastructure.Persistence;

namespace PortfolioERP.Infrastructure.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly AppDbContext _dbContext;

    public CustomerService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<CustomerResponse>> GetAllAsync(
        CustomerQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        ValidateQueryParameters(parameters);

        var query = _dbContext.Customers
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();

            query = query.Where(customer =>
                EF.Functions.ILike(customer.CustomerCode, $"%{search}%") ||
                EF.Functions.ILike(customer.CompanyName, $"%{search}%") ||
                EF.Functions.ILike(customer.Email, $"%{search}%") ||
                (
                    customer.FirstName != null &&
                    EF.Functions.ILike(customer.FirstName, $"%{search}%")
                ) ||
                (
                    customer.LastName != null &&
                    EF.Functions.ILike(customer.LastName, $"%{search}%")
                ) ||
                (
                    customer.TaxCode != null &&
                    EF.Functions.ILike(customer.TaxCode, $"%{search}%")
                ) ||
                (
                    customer.VatNumber != null &&
                    EF.Functions.ILike(customer.VatNumber, $"%{search}%")
                ));
        }

        if (!string.IsNullOrWhiteSpace(parameters.City))
        {
            var city = parameters.City.Trim();

            query = query.Where(customer =>
                customer.City != null &&
                EF.Functions.ILike(customer.City, city));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Country))
        {
            var country = parameters.Country.Trim();

            query = query.Where(customer =>
                EF.Functions.ILike(customer.Country, country));
        }

        if (parameters.IsActive.HasValue)
        {
            query = query.Where(customer =>
                customer.IsActive == parameters.IsActive.Value);
        }

        query = ApplySorting(
            query,
            parameters.SortBy,
            parameters.Descending);

        var totalItems = await query.CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)parameters.PageSize);

        var customers = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(customer => new CustomerResponse(
                customer.Id,
                customer.CustomerCode,
                customer.CompanyName,
                customer.FirstName,
                customer.LastName,
                customer.TaxCode,
                customer.VatNumber,
                customer.Email,
                customer.Phone,
                customer.Address,
                customer.City,
                customer.Province,
                customer.PostalCode,
                customer.Country,
                customer.IsActive,
                customer.CreatedAtUtc,
                customer.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResponse<CustomerResponse>(
            customers,
            parameters.PageNumber,
            parameters.PageSize,
            totalItems,
            totalPages);
    }

    public async Task<CustomerResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .Where(customer => customer.Id == id)
            .Select(customer => new CustomerResponse(
                customer.Id,
                customer.CustomerCode,
                customer.CompanyName,
                customer.FirstName,
                customer.LastName,
                customer.TaxCode,
                customer.VatNumber,
                customer.Email,
                customer.Phone,
                customer.Address,
                customer.City,
                customer.Province,
                customer.PostalCode,
                customer.Country,
                customer.IsActive,
                customer.CreatedAtUtc,
                customer.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        ValidateCustomer(
            request.CustomerCode,
            request.CompanyName,
            request.Email,
            request.Country);

        var customerCode = request.CustomerCode
            .Trim()
            .ToUpperInvariant();

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var taxCode = NormalizeOptional(request.TaxCode)?
            .ToUpperInvariant();

        var vatNumber = NormalizeOptional(request.VatNumber)?
            .ToUpperInvariant();

        await ValidateUniqueFieldsAsync(
            id: null,
            customerCode,
            taxCode,
            vatNumber,
            cancellationToken);

        var customer = new Customer
        {
            CustomerCode = customerCode,
            CompanyName = request.CompanyName.Trim(),
            FirstName = NormalizeOptional(request.FirstName),
            LastName = NormalizeOptional(request.LastName),
            TaxCode = taxCode,
            VatNumber = vatNumber,
            Email = email,
            Phone = NormalizeOptional(request.Phone),
            Address = NormalizeOptional(request.Address),
            City = NormalizeOptional(request.City),
            Province = NormalizeOptional(request.Province)?
                .ToUpperInvariant(),
            PostalCode = NormalizeOptional(request.PostalCode),
            Country = request.Country.Trim()
        };

        _dbContext.Customers.Add(customer);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(customer);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(
                customer => customer.Id == id,
                cancellationToken);

        if (customer is null)
        {
            return false;
        }

        ValidateCustomer(
            request.CustomerCode,
            request.CompanyName,
            request.Email,
            request.Country);

        var customerCode = request.CustomerCode
            .Trim()
            .ToUpperInvariant();

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var taxCode = NormalizeOptional(request.TaxCode)?
            .ToUpperInvariant();

        var vatNumber = NormalizeOptional(request.VatNumber)?
            .ToUpperInvariant();

        await ValidateUniqueFieldsAsync(
            id,
            customerCode,
            taxCode,
            vatNumber,
            cancellationToken);

        customer.CustomerCode = customerCode;
        customer.CompanyName = request.CompanyName.Trim();
        customer.FirstName = NormalizeOptional(request.FirstName);
        customer.LastName = NormalizeOptional(request.LastName);
        customer.TaxCode = taxCode;
        customer.VatNumber = vatNumber;
        customer.Email = email;
        customer.Phone = NormalizeOptional(request.Phone);
        customer.Address = NormalizeOptional(request.Address);
        customer.City = NormalizeOptional(request.City);
        customer.Province = NormalizeOptional(request.Province)?
            .ToUpperInvariant();
        customer.PostalCode = NormalizeOptional(request.PostalCode);
        customer.Country = request.Country.Trim();
        customer.IsActive = request.IsActive;
        customer.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(
                customer => customer.Id == id,
                cancellationToken);

        if (customer is null)
        {
            return false;
        }

        customer.IsActive = false;
        customer.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task ValidateUniqueFieldsAsync(
        int? id,
        string customerCode,
        string? taxCode,
        string? vatNumber,
        CancellationToken cancellationToken)
    {
        var duplicateCode = await _dbContext.Customers
            .AnyAsync(
                customer =>
                    (!id.HasValue || customer.Id != id.Value) &&
                    customer.CustomerCode == customerCode,
                cancellationToken);

        if (duplicateCode)
        {
            throw new ConflictException(
                "A customer with the same code already exists.");
        }

        if (taxCode is not null)
        {
            var duplicateTaxCode = await _dbContext.Customers
                .AnyAsync(
                    customer =>
                        (!id.HasValue || customer.Id != id.Value) &&
                        customer.TaxCode == taxCode,
                    cancellationToken);

            if (duplicateTaxCode)
            {
                throw new ConflictException(
                    "A customer with the same tax code already exists.");
            }
        }

        if (vatNumber is not null)
        {
            var duplicateVatNumber = await _dbContext.Customers
                .AnyAsync(
                    customer =>
                        (!id.HasValue || customer.Id != id.Value) &&
                        customer.VatNumber == vatNumber,
                    cancellationToken);

            if (duplicateVatNumber)
            {
                throw new ConflictException(
                    "A customer with the same VAT number already exists.");
            }
        }
    }

    private static void ValidateCustomer(
        string customerCode,
        string companyName,
        string email,
        string country)
    {
        if (string.IsNullOrWhiteSpace(customerCode))
        {
            throw new ValidationException(
                "Customer code is required.");
        }

        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new ValidationException(
                "Company name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ValidationException(
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ValidationException(
                "Country is required.");
        }
    }

    private static void ValidateQueryParameters(
        CustomerQueryParameters parameters)
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
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.CustomerCode,
            customer.CompanyName,
            customer.FirstName,
            customer.LastName,
            customer.TaxCode,
            customer.VatNumber,
            customer.Email,
            customer.Phone,
            customer.Address,
            customer.City,
            customer.Province,
            customer.PostalCode,
            customer.Country,
            customer.IsActive,
            customer.CreatedAtUtc,
            customer.UpdatedAtUtc);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static IQueryable<Customer> ApplySorting(
        IQueryable<Customer> query,
        string? sortBy,
        bool descending)
    {
        var normalizedSortBy = sortBy?
            .Trim()
            .ToLowerInvariant();

        return (normalizedSortBy, descending) switch
        {
            ("code", false) =>
                query.OrderBy(customer => customer.CustomerCode)
                    .ThenBy(customer => customer.Id),

            ("code", true) =>
                query.OrderByDescending(customer => customer.CustomerCode)
                    .ThenByDescending(customer => customer.Id),

            ("email", false) =>
                query.OrderBy(customer => customer.Email)
                    .ThenBy(customer => customer.Id),

            ("email", true) =>
                query.OrderByDescending(customer => customer.Email)
                    .ThenByDescending(customer => customer.Id),

            ("city", false) =>
                query.OrderBy(customer => customer.City)
                    .ThenBy(customer => customer.Id),

            ("city", true) =>
                query.OrderByDescending(customer => customer.City)
                    .ThenByDescending(customer => customer.Id),

            ("country", false) =>
                query.OrderBy(customer => customer.Country)
                    .ThenBy(customer => customer.Id),

            ("country", true) =>
                query.OrderByDescending(customer => customer.Country)
                    .ThenByDescending(customer => customer.Id),

            ("createdatutc", false) =>
                query.OrderBy(customer => customer.CreatedAtUtc)
                    .ThenBy(customer => customer.Id),

            ("createdatutc", true) =>
                query.OrderByDescending(customer => customer.CreatedAtUtc)
                    .ThenByDescending(customer => customer.Id),

            (_, false) =>
                query.OrderBy(customer => customer.CompanyName)
                    .ThenBy(customer => customer.Id),

            (_, true) =>
                query.OrderByDescending(customer => customer.CompanyName)
                    .ThenByDescending(customer => customer.Id)
        };
    }
}