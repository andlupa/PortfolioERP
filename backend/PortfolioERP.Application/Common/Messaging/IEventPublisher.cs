namespace PortfolioERP.Application.Common.Messaging;

public interface IEventPublisher
{
    Task PublishOrderShippedAsync(
        OrderShippedEvent message,
        CancellationToken cancellationToken = default);
}