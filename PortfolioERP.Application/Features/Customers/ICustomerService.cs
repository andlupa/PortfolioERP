using PortfolioERP.Application.Common;

namespace PortfolioERP.Application.Features.Customers;

public interface ICustomerService
{
    Task<PagedResponse<CustomerResponse>> GetAllAsync(
        CustomerQueryParameters parameters,
        CancellationToken cancellationToken);

    Task<CustomerResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}