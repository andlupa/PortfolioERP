using FluentValidation;

namespace PortfolioERP.Application.Features.Authentication;

public sealed class UpdateUserRoleRequestValidator
    : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleRequestValidator()
    {
        RuleFor(request => request.Role)
            .NotEmpty()
            .Must(role =>
                role == "Admin" ||
                role == "User")
            .WithMessage(
                "Role must be Admin or User.");
    }
}