using PortfolioERP.Application.Common;

namespace PortfolioERP.Application.Features.Suppliers;

public interface ISupplierService
{
    Task<PagedResponse<SupplierResponse>> GetAllAsync(
        SupplierQueryParameters parameters,
        CancellationToken cancellationToken);

    Task<SupplierResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<SupplierResponse> CreateAsync(
        CreateSupplierRequest request,
        CancellationToken cancellationToken);

    Task<SupplierResponse?> UpdateAsync(
        int id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeactivateAsync(
        int id,
        CancellationToken cancellationToken);
}