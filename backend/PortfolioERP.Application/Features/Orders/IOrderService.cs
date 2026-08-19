using PortfolioERP.Application.Common;

namespace PortfolioERP.Application.Features.Orders;

public interface IOrderService
{
    Task<PagedResponse<OrderListItemResponse>> GetAllAsync(
        OrderQueryParameters parameters,
        CancellationToken cancellationToken);

    Task<OrderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OrderCalculationResponse> CalculateAsync(
        CalculateOrderRequest request,
        CancellationToken cancellationToken);

    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken);

    Task<bool> CancelAsync(
        int id,
        CancellationToken cancellationToken);

    Task<bool> ConfirmAsync(
        int id,
        CancellationToken cancellationToken);

    Task<bool> ShipAsync(
        int id,
        CancellationToken cancellationToken);
}