using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Common.Exceptions;
using PortfolioERP.Application.Features.Categories;
using PortfolioERP.Domain.Entities;
using PortfolioERP.Infrastructure.Persistence;

namespace PortfolioERP.Infrastructure.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly AppDbContext _dbContext;

    public CategoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Categories
            .AsNoTracking()
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(category => category.IsActive);
        }

        return await query
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.Description,
                category.IsActive,
                category.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(category => category.Id == id)
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.Description,
                category.IsActive,
                category.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException(
                "Category name is required.");
        }

        var normalizedName = request.Name.Trim();

        var alreadyExists = await _dbContext.Categories
            .AnyAsync(
                category =>
                    category.Name.ToLower() ==
                    normalizedName.ToLower(),
                cancellationToken);

        if (alreadyExists)
        {
            throw new ConflictException(
                "A category with the same name already exists.");
        }

        var category = new Category
        {
            Name = normalizedName,
            Description = NormalizeOptional(request.Description)
        };

        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(category);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);

        if (category is null)
        {
            return false;
        }

        var normalizedName = request.Name.Trim();

        var duplicate = await _dbContext.Categories
            .AnyAsync(
                otherCategory =>
                    otherCategory.Id != id &&
                    otherCategory.Name.ToLower() ==
                    normalizedName.ToLower(),
                cancellationToken);

        if (duplicate)
        {
            throw new ConflictException(
                "Another category with the same name already exists.");
        }

        category.Name = normalizedName;
        category.Description =
            NormalizeOptional(request.Description);
        category.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);

        if (category is null)
        {
            return false;
        }

        category.IsActive = false;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.IsActive,
            category.CreatedAtUtc);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}