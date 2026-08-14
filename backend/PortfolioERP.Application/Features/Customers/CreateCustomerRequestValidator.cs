using FluentValidation;

namespace PortfolioERP.Application.Features.Customers;

public sealed class CreateCustomerRequestValidator
    : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.CustomerCode)
            .NotEmpty()
            .WithMessage("Customer code is required.")
            .Matches("^[A-Za-z0-9_-]+$")
            .WithMessage("Customer code contains invalid characters.")
            .MaximumLength(30)
            .WithMessage("Customer code cannot exceed 30 characters.");

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required.")
            .MaximumLength(200)
            .WithMessage("Company name cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email address.")
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .MaximumLength(30);

        RuleFor(x => x.Address)
            .MaximumLength(250);

        RuleFor(x => x.City)
            .MaximumLength(100);

        RuleFor(x => x.Province)
            .Length(2)
            .When(x => !string.IsNullOrWhiteSpace(x.Province))
            .WithMessage("Province must contain exactly 2 characters.");

        RuleFor(x => x.PostalCode)
            .Matches(@"^\d{5}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode))
            .WithMessage("Postal code must contain 5 digits.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .MaximumLength(100);

        RuleFor(x => x.TaxCode)
            .MaximumLength(30);

        RuleFor(x => x.VatNumber)
            .MaximumLength(30);
    }
}