using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Http;

using PortfolioERP.Application.Common;

namespace PortfolioERP.Infrastructure.Services;

public class InventoryClient : IInventoryClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InventoryClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task ReceiveAsync(
    int productId,
    int quantity,
    int purchaseOrderId,
    CancellationToken cancellationToken)
    {
        var body = new
        {
            ProductId = productId,
            Quantity = quantity,
            ReferenceType = "PurchaseOrder",
            ReferenceId = purchaseOrderId
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "api/inventory/receive")
        {
            Content = JsonContent.Create(body)
        };

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

        var responseBody =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        Console.WriteLine(
            $"InventoryService response: " +
            $"{(int)response.StatusCode} {response.StatusCode}");

        Console.WriteLine(
            $"InventoryService body: {responseBody}");

        response.EnsureSuccessStatusCode();
    }
}