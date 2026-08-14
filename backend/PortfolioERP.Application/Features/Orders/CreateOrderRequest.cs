namespace PortfolioERP.Application.Features.Orders;

public sealed record CreateOrderRequest(
    int CustomerId,
    string? Notes,
    IReadOnlyList<CreateOrderLineRequest> Lines);