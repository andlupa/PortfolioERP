namespace PortfolioERP.InventoryService.DTOs;

public record InventoryResponse(
    int ProductId,
    int QuantityOnHand,
    int QuantityReserved,
    int AvailableQuantity,
    int ReorderLevel);