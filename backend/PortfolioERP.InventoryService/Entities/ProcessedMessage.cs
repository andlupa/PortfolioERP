namespace PortfolioERP.InventoryService.Entities;

public class ProcessedMessage
{
    public int Id { get; set; }

    public string MessageType { get; set; } = null!;

    public string MessageId { get; set; } = null!;

    public DateTime ProcessedAtUtc { get; set; }
}