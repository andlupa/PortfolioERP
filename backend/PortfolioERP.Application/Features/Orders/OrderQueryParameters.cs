using PortfolioERP.Domain.Enums;

namespace PortfolioERP.Application.Features.Orders;

public sealed class OrderQueryParameters
{
	public string? Search { get; set; }

	public int? CustomerId { get; set; }

	public OrderStatus? Status { get; set; }

	public DateTime? DateFrom { get; set; }

	public DateTime? DateTo { get; set; }

	public decimal? MinTotal { get; set; }

	public decimal? MaxTotal { get; set; }

	public string SortBy { get; set; } = "orderDate";

	public bool Descending { get; set; } = true;

	public int PageNumber { get; set; } = 1;

	public int PageSize { get; set; } = 10;
}