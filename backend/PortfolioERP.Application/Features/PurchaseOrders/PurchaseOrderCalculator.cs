namespace PortfolioERP.Application.Features.PurchaseOrders;

public static class PurchaseOrderCalculator
{
    public static PurchaseOrderLineAmounts CalculateLine(
        int quantity,
        decimal unitPrice,
        decimal discountPercentage,
        decimal vatPercentage)
    {
        var grossAmount =
            quantity * unitPrice;

        var discountAmount =
            grossAmount * discountPercentage / 100m;

        var netAmount =
            grossAmount - discountAmount;

        var vatAmount =
            netAmount * vatPercentage / 100m;

        var totalAmount =
            netAmount + vatAmount;

        return new PurchaseOrderLineAmounts(
            decimal.Round(discountAmount, 2),
            decimal.Round(netAmount, 2),
            decimal.Round(vatAmount, 2),
            decimal.Round(totalAmount, 2));
    }
}

public record PurchaseOrderLineAmounts(
    decimal DiscountAmount,
    decimal NetAmount,
    decimal VatAmount,
    decimal TotalAmount
);