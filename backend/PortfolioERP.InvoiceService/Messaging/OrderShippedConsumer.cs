using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using PortfolioERP.InvoiceService.Domain;
using PortfolioERP.InvoiceService.Messaging.Contracts;
using PortfolioERP.InvoiceService.Persistence;

namespace PortfolioERP.InvoiceService.Messaging;

public sealed class OrderShippedConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    private IConnection? _connection;
    private IChannel? _channel;

    public OrderShippedConsumer(
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

        var factory = new ConnectionFactory
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
            queue: "invoice.order-shipped",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: "invoice.order-shipped",
            exchange: "portfolio.events",
            routingKey: "order.shipped",
            cancellationToken: stoppingToken);

        Console.WriteLine(
            "RabbitMQ OrderShipped consumer ready.");

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
                    JsonSerializer.Deserialize<OrderShippedEvent>(
                        json);

                if (message is null)
                {
                    throw new InvalidOperationException(
                        "Invalid OrderShipped message.");
                }

                await ProcessMessageAsync(
                    message,
                    stoppingToken);

                // Solo dopo il salvataggio della fattura
                // confermiamo il messaggio a RabbitMQ.
                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing OrderShipped: {ex.Message}");

                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "invoice.order-shipped",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    private async Task ProcessMessageAsync(
        OrderShippedEvent message,
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<InvoiceDbContext>();

        // Idempotenza:
        // lo stesso ordine non deve generare due fatture.
        var alreadyExists =
            await dbContext.Invoices
                .AnyAsync(
                    x => x.SalesOrderId == message.OrderId,
                    cancellationToken);

        if (alreadyExists)
        {
            Console.WriteLine(
                $"Invoice for SalesOrder {message.OrderId} " +
                "already exists. Skipping.");

            return;
        }

        var invoice = new Invoice
        {
            InvoiceNumber =
                $"INV-{message.OrderId:D6}",

            SalesOrderId = message.OrderId,

            CustomerId = message.CustomerId,
            CustomerName = message.CustomerName,

            InvoiceDateUtc = message.ShippedAtUtc,

            Status = InvoiceStatus.Issued,

            Subtotal = message.Subtotal,
            DiscountAmount = message.DiscountAmount,
            TaxAmount = message.TaxAmount,
            TotalAmount = message.TotalAmount,

            CreatedAtUtc = DateTime.UtcNow
        };

        foreach (var line in message.Lines)
        {
            invoice.Lines.Add(
                new InvoiceLine
                {
                    ProductId = line.ProductId,
                    ProductCode = line.ProductCode,
                    Description = line.ProductName,

                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,

                    DiscountPercentage =
                        line.DiscountPercentage,

                    DiscountAmount =
                        line.DiscountAmount,

                    NetAmount =
                        line.NetAmount,

                    VatPercentage =
                        line.VatPercentage,

                    VatAmount =
                        line.VatAmount,

                    TotalAmount =
                        line.TotalAmount
                });
        }

        dbContext.Invoices.Add(invoice);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        Console.WriteLine(
            $"Invoice {invoice.InvoiceNumber} created " +
            $"for SalesOrder {message.OrderId}.");
    }
}