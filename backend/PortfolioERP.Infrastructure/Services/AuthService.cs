using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Features.Authentication;
using PortfolioERP.Infrastructure.Persistence;

namespace PortfolioERP.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
	private readonly AppDbContext _dbContext;
	private readonly IPasswordService _passwordService;
	private readonly ITokenService _tokenService;

	public AuthService(
		AppDbContext dbContext,
		IPasswordService passwordService,
		ITokenService tokenService)
	{
		_dbContext = dbContext;
		_passwordService = passwordService;
		_tokenService = tokenService;
	}

	public async Task<LoginResponse?> LoginAsync(
		LoginRequest request,
		CancellationToken cancellationToken)
	{
		var normalizedUsername =
			request.Username.Trim().ToLowerInvariant();

		var user = await _dbContext.AppUsers
			.AsNoTracking()
			.FirstOrDefaultAsync(
				user =>
					user.Username.ToLower() ==
					normalizedUsername,
				cancellationToken);

		if (user is null || !user.IsActive)
		{
			return null;
		}

		var validPassword =
			_passwordService.VerifyPassword(
				request.Password,
				user.PasswordHash);

		if (!validPassword)
		{
			return null;
		}

		var expiresAtUtc =
			DateTime.UtcNow.AddHours(1);

		var accessToken =
			_tokenService.CreateToken(
				user,
				expiresAtUtc);

		return new LoginResponse(
			accessToken,
			expiresAtUtc,
			new UserResponse(
				user.Id,
				user.Username,
				user.Email,
				user.FirstName,
				user.LastName,
				user.Role));
	}
}