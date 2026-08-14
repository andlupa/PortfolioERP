using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public int SupplierId { get; set; }

    public DateTime OrderDate { get; set; }

    public PurchaseOrderStatus Status { get; set; }

    public decimal NetAmount { get; set; }

    public decimal VatAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public Supplier Supplier { get; set; } = null!;

    public ICollection<PurchaseOrderLine> Lines { get; set; }
        = new List<PurchaseOrderLine>();
}