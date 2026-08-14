using PortfolioERP.Application.Features.PurchaseOrders;

namespace PortfolioERP.Application.Features.PurchaseOrders;

public interface IPurchaseOrderService
{
    Task<IReadOnlyList<PurchaseOrderListResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PurchaseOrderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PurchaseOrderResponse> CreateAsync(
        CreatePurchaseOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<PurchaseOrderResponse> MarkAsOrderedAsync(
    int id,
    CancellationToken cancellationToken = default);

    Task<PurchaseOrderResponse> ReceiveAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PurchaseOrderResponse> CancelAsync(
        int id,
        CancellationToken cancellationToken = default);
}