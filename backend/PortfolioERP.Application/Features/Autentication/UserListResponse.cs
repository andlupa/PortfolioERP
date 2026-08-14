namespace PortfolioERP.Application.Features.Authentication;

public sealed record UserListResponse(
    int Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive,
    DateTime CreatedAt);