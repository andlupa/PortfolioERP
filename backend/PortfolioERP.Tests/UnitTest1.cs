using PortfolioERP.Domain.Services.Orders;

namespace PortfolioERP.Tests.Orders;

public class OrderCalculatorTests
{
    private readonly OrderCalculator _calculator = new();

    [Fact]
    public void Calculate_WithDiscountAndVat_ReturnsCorrectTotals()
    {
        var lines = new[]
        {
            new OrderLineCalculationInput(
                ProductId: 1,
                Quantity: 2,
                UnitPrice: 100m,
                DiscountPercentage: 10m,
                VatPercentage: 22m)
        };

        var result = _calculator.Calculate(lines);

        Assert.Equal(200m, result.Subtotal);
        Assert.Equal(20m, result.DiscountAmount);
        Assert.Equal(39.60m, result.TaxAmount);
        Assert.Equal(219.60m, result.TotalAmount);
    }
}