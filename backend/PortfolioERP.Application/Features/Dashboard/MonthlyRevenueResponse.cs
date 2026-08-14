namespace PortfolioERP.Application.Features.Dashboard;

public sealed record MonthlyRevenueResponse(
    int Year,
    int Month,
    decimal Revenue);