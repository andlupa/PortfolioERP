using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using PortfolioERP.InventoryService.Domain;
using PortfolioERP.InventoryService.Persistence;

namespace PortfolioERP.InventoryService.Messaging;

public class ProductCreatedConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    private IConnection? _connection;
    private IChannel? _channel;

    public ProductCreatedConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var host =
            _configuration["RabbitMq:Host"]
            ?? throw new InvalidOperationException(
                "RabbitMq:Host is not configured.");

        var factory =
            new ConnectionFactory
            {
                HostName = host
            };

        _connection =
            await factory.CreateConnectionAsync(
                stoppingToken);

        _channel =
            await _connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(
            exchange: "portfolio.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: "inventory.product-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: "inventory.product-created",
            exchange: "portfolio.events",
            routingKey: "product.created",
            cancellationToken: stoppingToken);

        Console.WriteLine(
            "RabbitMQ ProductCreated consumer ready.");

        var consumer =
            new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json =
                    Encoding.UTF8.GetString(
                        ea.Body.ToArray());

                var message =
                    JsonSerializer.Deserialize<ProductCreatedEvent>(
                        json);

                if (message is null)
                {
                    throw new InvalidOperationException(
                        "Invalid ProductCreated message.");
                }

                using var scope =
                    _scopeFactory.CreateScope();

                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<InventoryDbContext>();

                var alreadyExists =
                    await dbContext.InventoryItems
                        .AnyAsync(
                            x => x.ProductId == message.ProductId,
                            stoppingToken);

                if (!alreadyExists)
                {
                    dbContext.InventoryItems.Add(
                        new InventoryItem
                        {
                            ProductId = message.ProductId,
                            QuantityOnHand = 0,
                            QuantityReserved = 0,
                            ReorderLevel = 0
                        });

                    await dbContext.SaveChangesAsync(
                        stoppingToken);

                    Console.WriteLine(
                        $"Inventory created for Product {message.ProductId}");
                }
                else
                {
                    Console.WriteLine(
                        $"Inventory for Product {message.ProductId} already exists.");
                }

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing ProductCreated: {ex.Message}");

                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "inventory.product-created",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}