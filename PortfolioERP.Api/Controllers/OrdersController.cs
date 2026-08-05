using Microsoft.AspNetCore.Mvc;
using PortfolioERP.Application.Common;
using PortfolioERP.Application.Features.Orders;

namespace PortfolioERP.Api.Controllers;

[ApiController]
[Route("api/orders")]
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
}