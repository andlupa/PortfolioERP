using PortfolioERP.InvoiceService.DTOs;

namespace PortfolioERP.InvoiceService.Services;

public interface IInvoiceService
{
    Task<IReadOnlyCollection<InvoiceResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<InvoiceResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<InvoiceResponse?> GetBySalesOrderIdAsync(
        int salesOrderId,
        CancellationToken cancellationToken = default);
}