using Microsoft.EntityFrameworkCore;

using PortfolioERP.InvoiceService.DTOs;
using PortfolioERP.InvoiceService.Persistence;

namespace PortfolioERP.InvoiceService.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly InvoiceDbContext _dbContext;

    public InvoiceService(
        InvoiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<InvoiceResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invoices
            .AsNoTracking()
            .OrderByDescending(x => x.InvoiceDateUtc)
            .ThenByDescending(x => x.Id)
            .Select(x => new InvoiceResponse(
                x.Id,
                x.InvoiceNumber,
                x.SalesOrderId,
                x.CustomerId,
                x.CustomerName,
                x.InvoiceDateUtc,
                x.Status,
                x.Subtotal,
                x.DiscountAmount,
                x.TaxAmount,
                x.TotalAmount,
                x.Lines
                    .OrderBy(line => line.Id)
                    .Select(line =>
                        new InvoiceLineResponse(
                            line.Id,
                            line.ProductId,
                            line.ProductCode,
                            line.Description,
                            line.Quantity,
                            line.UnitPrice,
                            line.DiscountPercentage,
                            line.DiscountAmount,
                            line.NetAmount,
                            line.VatPercentage,
                            line.VatAmount,
                            line.TotalAmount))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }

    public async Task<InvoiceResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invoices
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new InvoiceResponse(
                x.Id,
                x.InvoiceNumber,
                x.SalesOrderId,
                x.CustomerId,
                x.CustomerName,
                x.InvoiceDateUtc,
                x.Status,
                x.Subtotal,
                x.DiscountAmount,
                x.TaxAmount,
                x.TotalAmount,
                x.Lines
                    .OrderBy(line => line.Id)
                    .Select(line =>
                        new InvoiceLineResponse(
                            line.Id,
                            line.ProductId,
                            line.ProductCode,
                            line.Description,
                            line.Quantity,
                            line.UnitPrice,
                            line.DiscountPercentage,
                            line.DiscountAmount,
                            line.NetAmount,
                            line.VatPercentage,
                            line.VatAmount,
                            line.TotalAmount))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InvoiceResponse?> GetBySalesOrderIdAsync(
        int salesOrderId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invoices
            .AsNoTracking()
            .Where(x => x.SalesOrderId == salesOrderId)
            .Select(x => new InvoiceResponse(
                x.Id,
                x.InvoiceNumber,
                x.SalesOrderId,
                x.CustomerId,
                x.CustomerName,
                x.InvoiceDateUtc,
                x.Status,
                x.Subtotal,
                x.DiscountAmount,
                x.TaxAmount,
                x.TotalAmount,
                x.Lines
                    .OrderBy(line => line.Id)
                    .Select(line =>
                        new InvoiceLineResponse(
                            line.Id,
                            line.ProductId,
                            line.ProductCode,
                            line.Description,
                            line.Quantity,
                            line.UnitPrice,
                            line.DiscountPercentage,
                            line.DiscountAmount,
                            line.NetAmount,
                            line.VatPercentage,
                            line.VatAmount,
                            line.TotalAmount))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

}