namespace PortfolioERP.Application.Features.PurchaseOrders;

public record PurchaseOrderLineResponse(
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
    decimal TotalAmount
);