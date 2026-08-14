using FluentValidation;

namespace PortfolioERP.Application.Features.Orders;

public sealed class CalculateOrderRequestValidator
    : AbstractValidator<CalculateOrderRequest>
{
    public CalculateOrderRequestValidator()
    {
        RuleFor(request => request.Lines)
            .NotNull()
            .WithMessage("Order lines are required.")
            .NotEmpty()
            .WithMessage(
                "The order must contain at least one line.");

        RuleForEach(request => request.Lines)
            .SetValidator(
                new CalculateOrderLineRequestValidator());

        RuleFor(request => request.Lines)
            .Must(HaveUniqueProducts)
            .When(request => request.Lines is not null)
            .WithMessage(
                "Each product can appear only once in an order.");
    }

    private static bool HaveUniqueProducts(
        IReadOnlyList<CalculateOrderLineRequest> lines)
    {
        return lines
            .Select(line => line.ProductId)
            .Distinct()
            .Count() == lines.Count;
    }
}