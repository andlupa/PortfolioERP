namespace PortfolioERP.Domain.Services.Orders;

public interface IOrderCalculator
{
    OrderCalculationResult Calculate(
        IEnumerable<OrderLineCalculationInput> lines);
}