namespace PortfolioERP.Application.Features.Authentication;

public sealed record LoginRequest(
    string Username,
    string Password);