using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;

using PortfolioERP.Application.Common.Messaging;

namespace PortfolioERP.Infrastructure.Services;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqEventPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishGoodsReceivedAsync(
        GoodsReceivedEvent message,
        CancellationToken cancellationToken)
    {
        var host =
            _configuration["RabbitMq:Host"]
            ?? throw new InvalidOperationException(
                "RabbitMq:Host is not configured.");

        Console.WriteLine($"RabbitMQ host: {host}");

        var factory = new ConnectionFactory
        {
            HostName = host
        };

        Console.WriteLine("Opening RabbitMQ connection...");

        await using var connection =
            await factory.CreateConnectionAsync(cancellationToken);

        Console.WriteLine("RabbitMQ connection OPEN.");

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        Console.WriteLine("RabbitMQ channel OPEN.");

        await channel.ExchangeDeclareAsync(
            exchange: "portfolio.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        Console.WriteLine("Exchange portfolio.events DECLARED.");

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: "portfolio.events",
            routingKey: "goods.received",
            body: body,
            cancellationToken: cancellationToken);

        Console.WriteLine("Message goods.received PUBLISHED.");
    }

    public async Task PublishProductCreatedAsync(
        ProductCreatedEvent message,
        CancellationToken cancellationToken)
    {
        var host =
            _configuration["RabbitMq:Host"]
            ?? throw new InvalidOperationException(
                "RabbitMq:Host is not configured.");

        var factory = new ConnectionFactory
        {
            HostName = host
        };

        await using var connection =
            await factory.CreateConnectionAsync(
                cancellationToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: "portfolio.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var json =
            JsonSerializer.Serialize(message);

        var body =
            Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: "portfolio.events",
            routingKey: "product.created",
            body: body,
            cancellationToken: cancellationToken);

        Console.WriteLine(
            $"ProductCreated published for Product {message.ProductId}");
    }
}