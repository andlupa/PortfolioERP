namespace PortfolioERP.Application.Features.Orders;

public sealed record CalculateOrderLineRequest(
    int ProductId,
    int Quantity,
    decimal DiscountPercentage);