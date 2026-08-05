namespace PortfolioERP.Application.Features.Orders;

public sealed record OrderLineResponse(
    int Id,
    int ProductId,
    string ProductCode,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage,
    decimal DiscountAmount,
    decimal NetAmount,
    decimal VatPercentage,
    decimal VatAmount,
    decimal TotalAmount);