namespace PortfolioERP.InvoiceService.DTOs;

public sealed record InvoiceLineResponse(
    int Id,
    int ProductId,
    string ProductCode,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage,
    decimal DiscountAmount,
    decimal NetAmount,
    decimal VatPercentage,
    decimal VatAmount,
    decimal TotalAmount);