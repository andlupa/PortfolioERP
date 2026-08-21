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

    public async Task PublishOrderShippedAsync(
        OrderShippedEvent message,
        CancellationToken cancellationToken = default)
    {
        // Legge la configurazione per l'host di RabbitMQ
        var connectionString =
            _configuration["RabbitMq:ConnectionString"]
            ?? throw new InvalidOperationException(
                "RabbitMq:ConnectionString is not configured.");

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        Console.WriteLine("Opening RabbitMQ connection...");

        // Apre la connessione e il canale in modalità asincrona
        await using var connection =
            await factory.CreateConnectionAsync(cancellationToken);

        Console.WriteLine("RabbitMQ connection OPEN.");

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        Console.WriteLine("RabbitMQ channel OPEN.");

        // Dichiarazione dell'exchange "portfolio.events" di tipo Topic
        await channel.ExchangeDeclareAsync(
            exchange: "portfolio.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        Console.WriteLine("Exchange portfolio.events DECLARED.");

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        // Pubblica il messaggio sull'exchange con la routing key "order.shipped"
        await channel.BasicPublishAsync(
            exchange: "portfolio.events",
            routingKey: "order.shipped",
            body: body,
            cancellationToken: cancellationToken);
    }
}