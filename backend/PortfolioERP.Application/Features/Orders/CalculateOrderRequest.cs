namespace PortfolioERP.Application.Features.Orders;

public sealed record CalculateOrderRequest(
    IReadOnlyList<CalculateOrderLineRequest> Lines);