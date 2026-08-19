namespace PortfolioERP.InventoryService.DTOs;

public record ShipInventoryRequest(
    int ProductId,
    int Quantity,
    string? ReferenceType,
    int? ReferenceId);