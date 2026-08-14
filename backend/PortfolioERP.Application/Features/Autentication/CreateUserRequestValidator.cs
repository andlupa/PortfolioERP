using FluentValidation;

namespace PortfolioERP.Application.Features.Authentication;

public sealed class CreateUserRequestValidator
    : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
                .WithMessage(
                    "Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
                .WithMessage(
                    "Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
                .WithMessage(
                    "Password must contain at least one number.");

        RuleFor(request => request.Role)
            .NotEmpty()
            .Must(role =>
                role == "Admin" ||
                role == "User")
            .WithMessage(
                "Role must be Admin or User.");
    }
}