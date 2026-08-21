namespace PortfolioERP.InvoiceService.Domain;

public class Invoice
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public int SalesOrderId { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public DateTime InvoiceDateUtc { get; set; }

    public InvoiceStatus Status { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public ICollection<InvoiceLine> Lines { get; set; }
        = new List<InvoiceLine>();
}