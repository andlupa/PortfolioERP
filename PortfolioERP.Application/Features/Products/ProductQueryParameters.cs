namespace PortfolioERP.Application.Features.Products;

public sealed class ProductQueryParameters
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public bool? IsActive { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string SortBy { get; set; } = "name";

    public bool Descending { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}