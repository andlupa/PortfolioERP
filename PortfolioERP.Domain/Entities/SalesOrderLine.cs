namespace PortfolioERP.Domain.Entities;

public class SalesOrderLine
{
    public int Id { get; set; }

    // Ordine

    public int SalesOrderId { get; set; }

    public SalesOrder SalesOrder { get; set; } = null!;

    // Prodotto

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    // Dati economici

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal NetAmount { get; set; }

    public decimal VatPercentage { get; set; }

    public decimal VatAmount { get; set; }

    public decimal TotalAmount { get; set; }


}