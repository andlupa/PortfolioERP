namespace PortfolioERP.Application.Features.Dashboard;

public sealed record LowStockProductResponse(
    int Id,
    string Code,
    string Name,
    int StockQuantity);