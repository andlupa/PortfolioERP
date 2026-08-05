using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Application.Features.Orders;

public sealed record OrderResponse(
    int Id,
    string OrderNumber,
    DateTime OrderDate,
    OrderStatus Status,
    int CustomerId,
    string CustomerName,
    string? Notes,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyList<OrderLineResponse> Lines);