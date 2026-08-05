namespace PortfolioERP.Application.Features.Products;

public sealed record ProductResponse(
    int Id,
    string Code,
    string Name,
    string? Description,
    decimal Price,
    decimal VatPercentage,
    int StockQuantity,
    bool IsActive,
    DateTime CreatedAtUtc,
    int CategoryId,
    string CategoryName);