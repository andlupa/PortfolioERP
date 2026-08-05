namespace PortfolioERP.Application.Features.Categories;

public sealed record UpdateCategoryRequest(
    string Name,
    string? Description,
    bool IsActive);