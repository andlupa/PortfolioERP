using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortfolioERP.InventoryService.DTOs;
using PortfolioERP.InventoryService.Services;

namespace PortfolioERP.InventoryService.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(
        IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetByProductId(
        int productId,
        CancellationToken cancellationToken)
    {
        var inventory =
            await _inventoryService.GetByProductIdAsync(
                productId,
                cancellationToken);

        if (inventory is null)
            return NotFound();

        return Ok(inventory);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var inventory =
            await _inventoryService.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetByProductId),
            new { productId = inventory.ProductId },
            inventory);
    }

    [HttpPost("receive")]
    public async Task<IActionResult> Receive(
    ReceiveInventoryRequest request,
    CancellationToken cancellationToken)
    {
        var inventory =
            await _inventoryService.ReceiveAsync(
                request,
                cancellationToken);

        return Ok(inventory);
    }

    [HttpPost("reserve")]
    public async Task<IActionResult> Reserve(
    ReserveInventoryRequest request,
    CancellationToken cancellationToken)
    {
        var inventory =
            await _inventoryService.ReserveAsync(
                request,
                cancellationToken);

        return Ok(inventory);
    }

    [HttpPost("release")]
    public async Task<IActionResult> Release(
    ReleaseInventoryRequest request,
    CancellationToken cancellationToken)
    {
        var inventory =
            await _inventoryService.ReleaseAsync(
                request,
                cancellationToken);

        return Ok(inventory);
    }

    [HttpPost("ship")]
    public async Task<IActionResult> Ship(
    ShipInventoryRequest request,
    CancellationToken cancellationToken)
    {
        var inventory =
            await _inventoryService.ShipAsync(
                request,
                cancellationToken);

        return Ok(inventory);
    }
}