namespace PortfolioERP.Application.Features.Dashboard;

public sealed record DashboardResponse(
    int ActiveProducts,
    int ActiveCustomers,
    int TotalOrders,
    int OrdersThisMonth,
    decimal RevenueThisMonth,
    IReadOnlyList<OrderStatusSummaryResponse> OrdersByStatus,
    IReadOnlyList<RecentOrderResponse> RecentOrders,
    IReadOnlyList<LowStockProductResponse> LowStockProducts,
    IReadOnlyList<MonthlyRevenueResponse> MonthlyRevenue);