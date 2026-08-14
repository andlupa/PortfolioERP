using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortfolioERP.Application.Features.Dashboard;

namespace PortfolioERP.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(DashboardResponse),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponse>> Get(
        CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardService.GetAsync(
            cancellationToken);

        return Ok(dashboard);
    }
}