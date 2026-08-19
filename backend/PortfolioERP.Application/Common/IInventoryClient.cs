namespace PortfolioERP.Application.Common;

public interface IInventoryClient
{
    Task ReceiveAsync(
        int productId,
        int quantity,
        int purchaseOrderId,
        CancellationToken cancellationToken);
}