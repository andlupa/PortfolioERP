namespace PortfolioERP.InventoryService.DTOs;

public record ReleaseInventoryRequest(
    int ProductId,
    int Quantity,
    string? ReferenceType,
    int? ReferenceId);