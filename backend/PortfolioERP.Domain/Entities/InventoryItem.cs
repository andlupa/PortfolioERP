namespace PortfolioERP.Domain.Entities;

public class InventoryItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int QuantityOnHand { get; set; }

    public int QuantityReserved { get; set; }

    public int ReorderLevel { get; set; }

    public int AvailableQuantity =>
        QuantityOnHand - QuantityReserved;

    public Product Product { get; set; } = null!;
}