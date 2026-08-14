using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Application.Features.PurchaseOrders;

public record PurchaseOrderListResponse(
    int Id,
    string OrderNumber,
    int SupplierId,
    string SupplierCompanyName,
    DateTime OrderDate,
    PurchaseOrderStatus Status,
    decimal TotalAmount
);