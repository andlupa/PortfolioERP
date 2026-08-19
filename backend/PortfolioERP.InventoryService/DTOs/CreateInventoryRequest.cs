namespace PortfolioERP.InventoryService.DTOs;

public record CreateInventoryRequest(
    int ProductId,
    int InitialQuantity,
    int ReorderLevel);