namespace PortfolioERP.Application.Features.Categories;

public sealed record CategoryResponse(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc);