using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Application.Features.Orders;

public sealed record OrderListItemResponse(
    int Id,
    string OrderNumber,
    DateTime OrderDate,
    OrderStatus Status,
    int CustomerId,
    string CustomerName,
    int LineCount,
    decimal TotalAmount);