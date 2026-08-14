using FluentValidation;

namespace PortfolioERP.Application.Features.Suppliers;

public sealed class UpdateSupplierRequestValidator
    : AbstractValidator<UpdateSupplierRequest>
{
    public UpdateSupplierRequestValidator()
    {
        RuleFor(x => x.SupplierCode)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);
    }
}