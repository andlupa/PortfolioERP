namespace PortfolioERP.Application.Features.Authentication;

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserResponse User);