namespace PortfolioERP.Application.Features.Products;

public record ProductResponse(
    int Id,
    string Code,
    string Name,
    string? Description,
    decimal Price,
    decimal VatPercentage,
    int QuantityOnHand,
    int QuantityReserved,
    int AvailableQuantity,
    bool IsActive,
    DateTime CreatedAtUtc,
    int CategoryId,
    string CategoryName);
