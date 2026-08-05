namespace PortfolioERP.Domain.Services.Orders;

public sealed record OrderCalculationResult(
	IReadOnlyList<OrderLineCalculationResult> Lines,
	decimal Subtotal,
	decimal DiscountAmount,
	decimal TaxAmount,
	decimal TotalAmount);