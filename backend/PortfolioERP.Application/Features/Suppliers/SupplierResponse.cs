namespace PortfolioERP.Application.Features.Suppliers;

public sealed record SupplierResponse(
    int Id,
    string SupplierCode,
    string CompanyName,
    string? ContactName,
    string? VatNumber,
    string? TaxCode,
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