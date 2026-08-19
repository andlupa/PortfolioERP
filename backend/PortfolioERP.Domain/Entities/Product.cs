namespace PortfolioERP.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal VatPercentage { get; set; } = 22m;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public InventoryItem? Inventory { get; set; }

    public ICollection<SalesOrderLine> OrderLines { get; set; }
    = new List<SalesOrderLine>();

    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    = new List<PurchaseOrderLine>();
}