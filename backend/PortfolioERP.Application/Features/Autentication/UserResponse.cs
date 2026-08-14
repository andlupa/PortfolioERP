namespace PortfolioERP.Application.Features.Authentication;

public sealed record UserResponse(
    int Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Role);