namespace PortfolioERP.Application.Features.Dashboard;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync(
        CancellationToken cancellationToken);
}