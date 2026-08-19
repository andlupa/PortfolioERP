namespace PortfolioERP.InventoryService.Messaging;

public record ProductCreatedEvent(
    int ProductId,
    string Code,
    string Name);