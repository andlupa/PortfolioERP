namespace PortfolioERP.Application.Features.Categories;

public sealed record CreateCategoryRequest(
    string Name,
    string? Description);