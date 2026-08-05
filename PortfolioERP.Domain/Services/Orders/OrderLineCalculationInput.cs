namespace PortfolioERP.Domain.Services.Orders;

public sealed record OrderLineCalculationInput(
    int ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage,
    decimal VatPercentage);