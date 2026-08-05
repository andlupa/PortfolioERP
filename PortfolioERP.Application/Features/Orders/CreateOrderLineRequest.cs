namespace PortfolioERP.Application.Features.Orders;

public sealed record CreateOrderLineRequest(
    int ProductId,
    int Quantity,
    decimal DiscountPercentage);