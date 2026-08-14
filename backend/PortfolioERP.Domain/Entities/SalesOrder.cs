using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Domain.Entities;

public class SalesOrder
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Draft;

    public string? Notes { get; set; }

    // Cliente

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    // Totali

    public decimal Subtotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    // Audit

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    // Navigazione

    public ICollection<SalesOrderLine> Lines { get; set; }
        = new List<SalesOrderLine>();
}