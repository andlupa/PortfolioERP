using FluentValidation;
using PortfolioERP.Application.Features.PurchaseOrders;

namespace PortfolioERP.Application.Features.PurchaseOrders;

public class PurchaseOrderLineRequestValidator
    : AbstractValidator<PurchaseOrderLineRequest>
{
    public PurchaseOrderLineRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.VatPercentage)
            .InclusiveBetween(0, 100);
    }
}