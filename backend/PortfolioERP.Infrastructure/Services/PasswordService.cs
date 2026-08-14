using Microsoft.AspNetCore.Identity;
using PortfolioERP.Application.Features.Authentication;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Services;

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(
            new AppUser(),
            password);
    }

    public bool VerifyPassword(
        string password,
        string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            new AppUser(),
            passwordHash,
            password);

        return result is
            PasswordVerificationResult.Success or
            PasswordVerificationResult.SuccessRehashNeeded;
    }
}