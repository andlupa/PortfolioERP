using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Application.Features.PurchaseOrders;

public record PurchaseOrderResponse(
    int Id,
    string OrderNumber,
    int SupplierId,
    string SupplierCode,
    string SupplierCompanyName,
    DateTime OrderDate,
    PurchaseOrderStatus Status,
    decimal NetAmount,
    decimal VatAmount,
    decimal TotalAmount,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyList<PurchaseOrderLineResponse> Lines
);