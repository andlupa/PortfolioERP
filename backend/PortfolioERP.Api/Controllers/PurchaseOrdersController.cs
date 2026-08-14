using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioERP.Application.Features.PurchaseOrders;

namespace PortfolioERP.Api.Controllers;

[ApiController]
[Route("api/purchaseorders")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _service;

    public PurchaseOrdersController(
        IPurchaseOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var orders =
            await _service.GetAllAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var order =
            await _service.GetByIdAsync(
                id,
                cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order =
            await _service.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    [HttpPost("{id:int}/order")]
    public async Task<IActionResult> MarkAsOrdered(
        int id,
        CancellationToken cancellationToken)
    {
        var order =
            await _service.MarkAsOrderedAsync(
                id,
                cancellationToken);

        return Ok(order);
    }

    [HttpPost("{id:int}/receive")]
    public async Task<IActionResult> Receive(
        int id,
        CancellationToken cancellationToken)
    {
        var order =
            await _service.ReceiveAsync(
                id,
                cancellationToken);

        return Ok(order);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        CancellationToken cancellationToken)
    {
        var order =
            await _service.CancelAsync(
                id,
                cancellationToken);

        return Ok(order);
    }
}