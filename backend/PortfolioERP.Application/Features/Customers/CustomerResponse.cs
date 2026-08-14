namespace PortfolioERP.Application.Features.Customers;

public sealed record CustomerResponse(
    int Id,
    string CustomerCode,
    string CompanyName,
    string? FirstName,
    string? LastName,
    string? TaxCode,
    string? VatNumber,
    string Email,
    string? Phone,
    string? Address,
    string? City,
    string? Province,
    string? PostalCode,
    string Country,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);