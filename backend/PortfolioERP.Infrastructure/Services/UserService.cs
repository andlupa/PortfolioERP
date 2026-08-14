using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Features.Authentication;
using PortfolioERP.Infrastructure.Persistence;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordService _passwordService;

    public UserService(
        AppDbContext dbContext,
        IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task<IReadOnlyList<UserListResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.AppUsers
            .AsNoTracking()
            .OrderBy(user => user.Username)
            .Select(user =>
                new UserListResponse(
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role,
                    user.IsActive,
                    user.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserListResponse> CreateAsync(
    CreateUserRequest request,
    CancellationToken cancellationToken)
    {
        var username =
            request.Username.Trim().ToLowerInvariant();

        var email =
            request.Email.Trim().ToLowerInvariant();

        var usernameExists =
            await _dbContext.AppUsers.AnyAsync(
                user => user.Username.ToLower() == username,
                cancellationToken);

        if (usernameExists)
        {
            throw new InvalidOperationException(
                "Username already exists.");
        }

        var emailExists =
            await _dbContext.AppUsers.AnyAsync(
                user => user.Email.ToLower() == email,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Email already exists.");
        }

        var role = request.Role.Trim();

        var user = new AppUser
        {
            Username = username,
            Email = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordService.HashPassword(
                request.Password);

        _dbContext.AppUsers.Add(user);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return new UserListResponse(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role,
            user.IsActive,
            user.CreatedAt);
    }

    public async Task<UserListResponse?> UpdateStatusAsync(
    int id,
    bool isActive,
    CancellationToken cancellationToken)
    {
        var user = await _dbContext.AppUsers
            .FirstOrDefaultAsync(
                user => user.Id == id,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.IsActive = isActive;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return MapUser(user);
    }

    public async Task<UserListResponse?> UpdateRoleAsync(
    int id,
    string role,
    CancellationToken cancellationToken)
    {
        var user = await _dbContext.AppUsers
            .FirstOrDefaultAsync(
                user => user.Id == id,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.Role = role.Trim();

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return MapUser(user);
    }

    private static UserListResponse MapUser(
    AppUser user)
    {
        return new UserListResponse(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role,
            user.IsActive,
            user.CreatedAt);
    }
}