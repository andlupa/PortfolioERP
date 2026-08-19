using System.Net;
using System.Net.Http.Headers;

namespace PortfolioERP.InventoryService.Clients;

public class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> ProductExistsAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/products/{productId}");

        var authorization =
            _httpContextAccessor.HttpContext?
                .Request.Headers.Authorization
                .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.Authorization =
                AuthenticationHeaderValue.Parse(authorization);
        }

        var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();

        return true;
    }
}