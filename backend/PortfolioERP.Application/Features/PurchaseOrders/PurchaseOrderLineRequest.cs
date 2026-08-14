namespace PortfolioERP.Application.Features.PurchaseOrders;

public record PurchaseOrderLineRequest(
    int ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage,
    decimal VatPercentage
);