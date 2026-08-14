using FluentValidation;

namespace PortfolioERP.Application.Features.Orders;

public sealed class CreateOrderLineRequestValidator
    : AbstractValidator<CreateOrderLineRequest>
{
    public CreateOrderLineRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage(
                "Discount percentage must be between 0 and 100.");
    }
}