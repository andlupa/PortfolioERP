using PortfolioERP.Application.Common;

namespace PortfolioERP.Application.Features.Products;

public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetAllAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken);

    Task<ProductResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}