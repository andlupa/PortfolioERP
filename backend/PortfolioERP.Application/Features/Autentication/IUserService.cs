namespace PortfolioERP.Application.Features.Authentication;

public interface IUserService
{
    Task<IReadOnlyList<UserListResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<UserListResponse> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken);

    Task<UserListResponse?> UpdateStatusAsync(
    int id,
    bool isActive,
    CancellationToken cancellationToken);

    Task<UserListResponse?> UpdateRoleAsync(
        int id,
        string role,
        CancellationToken cancellationToken);
}