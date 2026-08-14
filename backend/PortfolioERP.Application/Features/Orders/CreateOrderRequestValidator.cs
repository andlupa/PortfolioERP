using FluentValidation;

namespace PortfolioERP.Application.Features.Orders;

public sealed class CreateOrderRequestValidator
    : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("CustomerId must be greater than zero.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.");

        RuleFor(x => x.Lines)
            .NotNull()
            .WithMessage("Order lines are required.")
            .NotEmpty()
            .WithMessage(
                "The order must contain at least one line.");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreateOrderLineRequestValidator());

        RuleFor(x => x.Lines)
            .Must(HaveUniqueProducts)
            .When(x => x.Lines is not null)
            .WithMessage(
                "Each product can appear only once in an order.");
    }

    private static bool HaveUniqueProducts(
        IReadOnlyList<CreateOrderLineRequest> lines)
    {
        return lines
            .Select(line => line.ProductId)
            .Distinct()
            .Count() == lines.Count;
    }
}