namespace PortfolioERP.Application.Common.Messaging;

public record GoodsReceivedEvent(
    int PurchaseOrderId,
    IReadOnlyList<GoodsReceivedLine> Lines);

public record GoodsReceivedLine(
    int ProductId,
    int Quantity);