namespace PortfolioERP.Application.Features.Orders;

public sealed record OrderCalculationLineResponse(
    int ProductId,
    decimal UnitPrice,
    int Quantity,
    decimal GrossAmount,
    decimal DiscountPercentage,
    decimal DiscountAmount,
    decimal NetAmount,
    decimal VatPercentage,
    decimal VatAmount,
    decimal TotalAmount);