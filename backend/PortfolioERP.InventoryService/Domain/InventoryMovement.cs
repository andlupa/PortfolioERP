namespace PortfolioERP.InventoryService.Domain;

public class InventoryMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public InventoryMovementType Type { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public string? ReferenceType { get; set; }

    public int? ReferenceId { get; set; }
}