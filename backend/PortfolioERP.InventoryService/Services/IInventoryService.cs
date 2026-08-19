using PortfolioERP.InventoryService.DTOs;

namespace PortfolioERP.InventoryService.Services;

public interface IInventoryService
{
    Task<InventoryResponse?> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken);

    Task<InventoryResponse> CreateAsync(
        CreateInventoryRequest request,
        CancellationToken cancellationToken);

    Task<InventoryResponse> ReceiveAsync(
        ReceiveInventoryRequest request,
        CancellationToken cancellationToken);

    Task<InventoryResponse> ReserveAsync(
        ReserveInventoryRequest request,
        CancellationToken cancellationToken);

    Task<InventoryResponse> ReleaseAsync(
        ReleaseInventoryRequest request,
        CancellationToken cancellationToken);

    Task<InventoryResponse> ShipAsync(
        ShipInventoryRequest request,
        CancellationToken cancellationToken);
}