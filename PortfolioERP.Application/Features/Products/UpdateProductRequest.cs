namespace PortfolioERP.Application.Features.Products;

public sealed record UpdateProductRequest(
    string Code,
    string Name,
    string? Description,
    decimal Price,
    decimal VatPercentage,
    int StockQuantity,
    int CategoryId,
    bool IsActive);