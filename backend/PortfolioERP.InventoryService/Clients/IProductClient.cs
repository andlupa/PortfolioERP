namespace PortfolioERP.InventoryService.Clients;

public interface IProductClient
{
    Task<bool> ProductExistsAsync(
        int productId,
        CancellationToken cancellationToken);
}