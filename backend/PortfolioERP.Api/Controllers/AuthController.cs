using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioERP.Application.Features.Authentication;

namespace PortfolioERP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	[AllowAnonymous]
	[HttpPost("login")]
	[ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<LoginResponse>> Login(
		LoginRequest request,
		CancellationToken cancellationToken)
	{
		var response =
			await _authService.LoginAsync(
				request,
				cancellationToken);

		if (response is null)
		{
			return Unauthorized(new
			{
				message =
					"Invalid username or password."
			});
		}

		return Ok(response);
	}
}