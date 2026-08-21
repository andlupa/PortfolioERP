using PortfolioERP.InvoiceService.Domain;

namespace PortfolioERP.InvoiceService.DTOs;

public sealed record InvoiceResponse(
    int Id,
    string InvoiceNumber,
    int SalesOrderId,
    int CustomerId,
    string CustomerName,
    DateTime InvoiceDateUtc,
    InvoiceStatus Status,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    IReadOnlyCollection<InvoiceLineResponse> Lines);