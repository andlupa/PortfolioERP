namespace PortfolioERP.Application.Features.Products;

public sealed record CreateProductRequest(
    string Code,
    string Name,
    string? Description,
    decimal Price,
    decimal VatPercentage,
    int StockQuantity,
    int CategoryId);