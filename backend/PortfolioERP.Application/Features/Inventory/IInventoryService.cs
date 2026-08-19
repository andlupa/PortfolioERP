namespace PortfolioERP.Application.Features.Inventory;

public interface IInventoryService
{
    Task ReceiveAsync(
        int productId,
        int quantity,
        string referenceType,
        int? referenceId,
        CancellationToken cancellationToken = default);

    Task ReserveAsync(
        int productId,
        int quantity,
        string referenceType,
        int? referenceId,
        CancellationToken cancellationToken = default);

    Task ReleaseAsync(
        int productId,
        int quantity,
        string referenceType,
        int? referenceId,
        CancellationToken cancellationToken = default);

    Task ShipAsync(
        int productId,
        int quantity,
        string referenceType,
        int? referenceId,
        CancellationToken cancellationToken = default);

    Task<int> GetAvailableQuantityAsync(
        int productId,
        CancellationToken cancellationToken = default);
}