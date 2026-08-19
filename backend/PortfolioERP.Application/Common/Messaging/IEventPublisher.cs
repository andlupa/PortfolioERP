namespace PortfolioERP.Application.Common.Messaging;

public interface IEventPublisher
{
    Task PublishGoodsReceivedAsync(
        GoodsReceivedEvent message,
        CancellationToken cancellationToken);

    Task PublishProductCreatedAsync(
        ProductCreatedEvent message,
        CancellationToken cancellationToken);
}