namespace PortfolioERP.Application.Features.Suppliers;

public sealed class SupplierQueryParameters
{
    public string? Search { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public bool? IsActive { get; set; }

    public string SortBy { get; set; } = "companyName";

    public bool Descending { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}