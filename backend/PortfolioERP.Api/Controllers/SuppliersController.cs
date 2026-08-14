using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioERP.Application.Common;
using PortfolioERP.Application.Features.Suppliers;

namespace PortfolioERP.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
[Authorize]
public sealed class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(
        ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResponse<SupplierResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<SupplierResponse>>> GetAll(
        [FromQuery] SupplierQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var response =
            await _supplierService.GetAllAsync(
                parameters,
                cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(SupplierResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var supplier =
            await _supplierService.GetByIdAsync(
                id,
                cancellationToken);

        if (supplier is null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(SupplierResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SupplierResponse>> Create(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var supplier =
                await _supplierService.CreateAsync(
                    request,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = supplier.Id },
                supplier);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(
        typeof(SupplierResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierResponse>> Update(
        int id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var supplier =
                await _supplierService.UpdateAsync(
                    id,
                    request,
                    cancellationToken);

            if (supplier is null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _supplierService.DeactivateAsync(
                id,
                cancellationToken);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}