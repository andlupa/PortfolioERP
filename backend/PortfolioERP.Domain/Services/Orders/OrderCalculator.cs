namespace PortfolioERP.Domain.Services.Orders;

public sealed class OrderCalculator : IOrderCalculator
{
    private const int MoneyDecimals = 2;

    public OrderCalculationResult Calculate(
        IEnumerable<OrderLineCalculationInput> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var inputLines = lines.ToList();

        if (inputLines.Count == 0)
        {
            throw new ArgumentException(
                "The order must contain at least one line.",
                nameof(lines));
        }

        var calculatedLines = new List<OrderLineCalculationResult>();

        foreach (var line in inputLines)
        {
            ValidateLine(line);

            var grossAmount = RoundMoney(
                line.Quantity * line.UnitPrice);

            var discountAmount = RoundMoney(
                grossAmount * line.DiscountPercentage / 100m);

            var netAmount = RoundMoney(
                grossAmount - discountAmount);

            var vatAmount = RoundMoney(
                netAmount * line.VatPercentage / 100m);

            var totalAmount = RoundMoney(
                netAmount + vatAmount);

            calculatedLines.Add(
                new OrderLineCalculationResult(
                    line.ProductId,
                    line.Quantity,
                    line.UnitPrice,
                    line.DiscountPercentage,
                    discountAmount,
                    netAmount,
                    line.VatPercentage,
                    vatAmount,
                    totalAmount));
        }

        var subtotal = RoundMoney(
            calculatedLines.Sum(line =>
                line.Quantity * line.UnitPrice));

        var discountTotal = RoundMoney(
            calculatedLines.Sum(line =>
                line.DiscountAmount));

        var taxTotal = RoundMoney(
            calculatedLines.Sum(line =>
                line.VatAmount));

        var total = RoundMoney(
            calculatedLines.Sum(line =>
                line.TotalAmount));

        return new OrderCalculationResult(
            calculatedLines,
            subtotal,
            discountTotal,
            taxTotal,
            total);
    }

    private static void ValidateLine(
        OrderLineCalculationInput line)
    {
        if (line.ProductId <= 0)
        {
            throw new ArgumentException(
                "ProductId must be greater than zero.");
        }

        if (line.Quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        if (line.UnitPrice < 0)
        {
            throw new ArgumentException(
                "UnitPrice cannot be negative.");
        }

        if (line.DiscountPercentage is < 0 or > 100)
        {
            throw new ArgumentException(
                "DiscountPercentage must be between 0 and 100.");
        }

        if (line.VatPercentage is < 0 or > 100)
        {
            throw new ArgumentException(
                "VatPercentage must be between 0 and 100.");
        }
    }

    private static decimal RoundMoney(decimal value)
    {
        return Math.Round(
            value,
            MoneyDecimals,
            MidpointRounding.AwayFromZero);
    }
}