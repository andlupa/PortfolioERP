using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortfolioERP.Application.Common;
using PortfolioERP.Application.Features.Orders;

namespace PortfolioERP.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrdersController : ControllerBase
{
	private readonly IOrderService _orderService;

	public OrdersController(IOrderService orderService)
	{
		_orderService = orderService;
	}

	[HttpGet]
	public async Task<ActionResult<PagedResponse<OrderListItemResponse>>> GetAll(
		[FromQuery] OrderQueryParameters parameters,
		CancellationToken cancellationToken)
	{
		var orders = await _orderService.GetAllAsync(
			parameters,
			cancellationToken);

		return Ok(orders);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<OrderResponse>> GetById(
		int id,
		CancellationToken cancellationToken)
	{
		var order = await _orderService.GetByIdAsync(
			id,
			cancellationToken);

		if (order is null)
		{
			return NotFound();
		}

		return Ok(order);
	}

	[HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<OrderResponse>> Create(
		CreateOrderRequest request,
		CancellationToken cancellationToken)
	{
		var order = await _orderService.CreateAsync(
			request,
			cancellationToken);

		return CreatedAtAction(
			nameof(GetById),
			new { id = order.Id },
			order);
	}

	[HttpPost("{id:int}/confirm")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Confirm(
		int id,
		CancellationToken cancellationToken)
	{
		var confirmed = await _orderService.ConfirmAsync(
			id,
			cancellationToken);

		if (!confirmed)
		{
			return NotFound();
		}

		return NoContent();
	}

	[HttpPost("{id:int}/cancel")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Cancel(
		int id,
		CancellationToken cancellationToken)
	{
		var cancelled = await _orderService.CancelAsync(
			id,
			cancellationToken);

		if (!cancelled)
		{
			return NotFound();
		}

		return NoContent();
	}

    [HttpPost("calculate")]
    [ProducesResponseType(
    typeof(OrderCalculationResponse),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    typeof(ValidationProblemDetails),
    StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
    StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderCalculationResponse>> Calculate(
    CalculateOrderRequest request,
    CancellationToken cancellationToken)
    {
        var calculation = await _orderService.CalculateAsync(
            request,
            cancellationToken);

        return Ok(calculation);
    }

    [HttpPost("{id:int}/ship")]
    public async Task<IActionResult> Ship(
    int id,
    CancellationToken cancellationToken)
    {
        var result =
            await _orderService.ShipAsync(
                id,
                cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}