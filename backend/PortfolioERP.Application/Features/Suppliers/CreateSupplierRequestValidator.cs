using FluentValidation;

namespace PortfolioERP.Application.Features.Suppliers;

public sealed class CreateSupplierRequestValidator
    : AbstractValidator<CreateSupplierRequest>
{
    public CreateSupplierRequestValidator()
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

        RuleFor(x => x.VatNumber)
            .MaximumLength(20);

        RuleFor(x => x.TaxCode)
            .MaximumLength(20);
    }
}