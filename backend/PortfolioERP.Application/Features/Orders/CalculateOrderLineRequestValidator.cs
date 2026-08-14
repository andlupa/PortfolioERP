using FluentValidation;

namespace PortfolioERP.Application.Features.Orders;

public sealed class CalculateOrderLineRequestValidator
    : AbstractValidator<CalculateOrderLineRequest>
{
    public CalculateOrderLineRequestValidator()
    {
        RuleFor(line => line.ProductId)
            .GreaterThan(0)
            .WithMessage(
                "ProductId must be greater than zero.");

        RuleFor(line => line.Quantity)
            .GreaterThan(0)
            .WithMessage(
                "Quantity must be greater than zero.");

        RuleFor(line => line.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage(
                "DiscountPercentage must be between 0 and 100.");
    }
}