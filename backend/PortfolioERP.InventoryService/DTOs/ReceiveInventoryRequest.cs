namespace PortfolioERP.InventoryService.DTOs;

public record ReceiveInventoryRequest(
    int ProductId,
    int Quantity,
    string? ReferenceType,
    int? ReferenceId);