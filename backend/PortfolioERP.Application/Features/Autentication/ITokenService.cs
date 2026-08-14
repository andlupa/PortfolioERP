using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Application.Features.Authentication;

public interface ITokenService
{
    string CreateToken(
        AppUser user,
        DateTime expiresAtUtc);
}