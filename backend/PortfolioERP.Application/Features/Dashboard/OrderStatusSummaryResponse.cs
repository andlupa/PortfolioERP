using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Application.Features.Dashboard;

public sealed record OrderStatusSummaryResponse(
    OrderStatus Status,
    int Count);