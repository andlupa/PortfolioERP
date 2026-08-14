namespace PortfolioERP.Application.Features.PurchaseOrders;

public record CreatePurchaseOrderRequest(
    int SupplierId,
    DateTime OrderDate,
    string? Notes,
    IReadOnlyList<PurchaseOrderLineRequest> Lines
);