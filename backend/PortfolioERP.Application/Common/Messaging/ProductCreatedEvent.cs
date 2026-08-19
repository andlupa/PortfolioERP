namespace PortfolioERP.Application.Common.Messaging;

public record ProductCreatedEvent(
    int ProductId,
    string Code,
    string Name);
