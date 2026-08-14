using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PortfolioERP.Application.Features.Authentication;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Services;

public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(
        AppUser user,
        DateTime expiresAtUtc)
    {
        var keyValue =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "Jwt:Key is not configured.");

        var issuer =
            _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "Jwt:Issuer is not configured.");

        var audience =
            _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "Jwt:Audience is not configured.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,user.Id.ToString()),

            new(JwtRegisteredClaimNames.UniqueName,user.Username),

            new(JwtRegisteredClaimNames.Email,user.Email),

            new(ClaimTypes.NameIdentifier,user.Id.ToString()),

            new(ClaimTypes.Name,user.Username),

            new(ClaimTypes.Role,user.Role)
        };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyValue));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}