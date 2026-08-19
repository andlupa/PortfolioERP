namespace PortfolioERP.Application.Features.Dashboard;

public record LowStockProductResponse(
    int Id,
    string Code,
    string Name,
    int QuantityOnHand,
    int QuantityReserved,
    int AvailableQuantity,
    int ReorderLevel);