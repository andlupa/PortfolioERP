namespace PortfolioERP.Application.Features.Suppliers;

public sealed record CreateSupplierRequest(
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
    string Country);