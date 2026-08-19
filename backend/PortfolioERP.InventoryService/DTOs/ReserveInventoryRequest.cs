namespace PortfolioERP.InventoryService.DTOs;

public record ReserveInventoryRequest(
    int ProductId,
    int Quantity,
    string? ReferenceType,
    int? ReferenceId);