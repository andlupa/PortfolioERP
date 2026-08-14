namespace PortfolioERP.Application.Features.Authentication;

public sealed record CreateUserRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string Role);