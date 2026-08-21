namespace PortfolioERP.Application.Common.Messaging;

public sealed record OrderShippedEvent(
    int OrderId,
    string OrderNumber,
    DateTime ShippedAtUtc,
    int CustomerId,
    string CustomerName,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    IReadOnlyCollection<OrderShippedLineEvent> Lines);

public sealed record OrderShippedLineEvent(
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