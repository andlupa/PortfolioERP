namespace PortfolioERP.Application.Features.Orders;

public sealed record OrderCalculationResponse(
    IReadOnlyList<OrderCalculationLineResponse> Lines,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal NetAmount,
    decimal TaxAmount,
    decimal TotalAmount);