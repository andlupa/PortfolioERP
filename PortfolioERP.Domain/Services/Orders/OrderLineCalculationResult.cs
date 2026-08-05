namespace PortfolioERP.Domain.Services.Orders;

public sealed record OrderLineCalculationResult(
    int ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage,
    decimal DiscountAmount,
    decimal NetAmount,
    decimal VatPercentage,
    decimal VatAmount,
    decimal TotalAmount);