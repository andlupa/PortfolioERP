using FluentValidation;
using PortfolioERP.Domain.Security;

namespace PortfolioERP.Application.Features.Authentication;

public sealed class UpdateUserRoleRequestValidator
    : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleRequestValidator()
    {
        RuleFor(request => request.Role)
            .NotEmpty()
            .Must(role =>
                role == AppRoles.Admin ||
                role == AppRoles.User ||
                role == AppRoles.Demo)
            .WithMessage(
                "Role must be Admin, User or Demo.");
    }
}