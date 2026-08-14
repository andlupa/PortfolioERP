using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PortfolioERP.Application.Features.Authentication;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Persistence;

public sealed class DevelopmentDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _configuration;

    public DevelopmentDataSeeder(
        AppDbContext dbContext,
        IPasswordService passwordService,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _configuration = configuration;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var username =
            _configuration["SeedAdmin:Username"]
            ?? throw new InvalidOperationException("SeedAdmin:Username is not configured.");

        var email =
            _configuration["SeedAdmin:Email"]
            ?? throw new InvalidOperationException("SeedAdmin:Email is not configured.");

        var password =
            _configuration["SeedAdmin:Password"]
            ?? throw new InvalidOperationException("SeedAdmin:Password is not configured.");

        var adminExists = await _dbContext.AppUsers
            .AnyAsync(
                user => user.Username == username,
                cancellationToken);

        if (adminExists)
        {
            return;
        }

        var admin = new AppUser
        {
            Username = username,
            Email = email,
            FirstName = "System",
            LastName = "Administrator",
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        admin.PasswordHash = _passwordService.HashPassword(password);

        _dbContext.AppUsers.Add(admin);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}