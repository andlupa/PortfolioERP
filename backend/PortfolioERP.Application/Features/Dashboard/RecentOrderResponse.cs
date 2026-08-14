using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Application.Features.Dashboard;

public sealed record RecentOrderResponse(
    int Id,
    string OrderNumber,
    DateTime OrderDate,
    string CustomerName,
    OrderStatus Status,
    decimal TotalAmount);