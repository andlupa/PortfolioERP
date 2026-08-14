using FluentValidation;
using PortfolioERP.Application.Features.PurchaseOrders;

namespace PortfolioERP.Application.Features.PurchaseOrders;

public class CreatePurchaseOrderRequestValidator
    : AbstractValidator<CreatePurchaseOrderRequest>
{
    public CreatePurchaseOrderRequestValidator()
    {
        RuleFor(x => x.SupplierId)
            .GreaterThan(0);

        RuleFor(x => x.OrderDate)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .MaximumLength(1000);

        RuleFor(x => x.Lines)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Lines)
            .SetValidator(
                new PurchaseOrderLineRequestValidator());
    }
}