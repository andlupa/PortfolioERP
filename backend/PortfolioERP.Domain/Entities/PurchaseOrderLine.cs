namespace PortfolioERP.Domain.Entities;

public class PurchaseOrderLine
{
    public int Id { get; set; }

    public int PurchaseOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal NetAmount { get; set; }

    public decimal VatPercentage { get; set; }

    public decimal VatAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Product Product { get; set; } = null!;
}