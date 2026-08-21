using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using PortfolioERP.InvoiceService.DTOs;
using PortfolioERP.InvoiceService.Services;

namespace PortfolioERP.InvoiceService.Controllers;

[Authorize]
[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(
        IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<InvoiceResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var invoices =
            await _invoiceService.GetAllAsync(
                cancellationToken);

        return Ok(invoices);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var invoice =
            await _invoiceService.GetByIdAsync(
                id,
                cancellationToken);

        if (invoice is null)
        {
            return NotFound();
        }

        return Ok(invoice);
    }

    [HttpGet("by-order/{salesOrderId:int}")]
    public async Task<ActionResult<InvoiceResponse>> GetBySalesOrderId(
        int salesOrderId,
        CancellationToken cancellationToken)
    {
        var invoice =
            await _invoiceService.GetBySalesOrderIdAsync(
                salesOrderId,
                cancellationToken);

        if (invoice is null)
        {
            return NotFound();
        }

        return Ok(invoice);
    }
}