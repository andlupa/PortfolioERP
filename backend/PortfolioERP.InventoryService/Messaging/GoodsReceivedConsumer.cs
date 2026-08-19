using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using PortfolioERP.InventoryService.DTOs;
using PortfolioERP.InventoryService.Entities;
using PortfolioERP.InventoryService.Services;
using PortfolioERP.InventoryService.Persistence;

namespace PortfolioERP.InventoryService.Messaging;

// Gestisce l'evento RabbitQM del PurchaseOrder
// Rimane sempre in ascolto in background (BackgroundService)
public class GoodsReceivedConsumer : BackgroundService
{
    // Per leggere la configurazione RabbitMq:Host
    private readonly IConfiguration _configuration;
    // Scope per ID quando arriva il messaggio
    private readonly IServiceScopeFactory _scopeFactory;

    // Connessione TCP e Canale per comunicare con RabbitQm
    private IConnection? _connection;
    private IChannel? _channel;

    public GoodsReceivedConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    // BackgroundService chiama automaticamente
    // ExecuteAsync() quando l'applicazione parte
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        // Legge l'host dalla configurazione (es. Localhost)
        var host =
            _configuration["RabbitMq:Host"]
            ?? throw new InvalidOperationException(
                "RabbitMq:Host is not configured.");

        // Oggetto per la connessione
        var factory = new ConnectionFactory { HostName = host };

        // Apriamo la connessione
        _connection = await factory.CreateConnectionAsync(stoppingToken);

        // Apriamo il canale di connessione
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // Dichiara l'Exchange portfolio.events
        await _channel.ExchangeDeclareAsync(
            exchange: "portfolio.events",
            type: ExchangeType.Topic,
            durable: true, // sopravvive al riavvio di RabbitQM
            autoDelete: false, // Non deve essere cancellato in assenza di consumer
            cancellationToken: stoppingToken);

        // Creiamo la coda di attesa del messaggio
        await _channel.QueueDeclareAsync(
            queue: "inventory.goods-received",
            durable: true, // sopravvive al riavvio di RabbitQM
            exclusive: false, // non appartiene esclusivamente a questa singola connessione
            autoDelete: false, // Non deve essere cancellato in assenza di consumer
            cancellationToken: stoppingToken);

        // dice a Rabbit QM di mettere nella queue inventory.goods-received
        // il messaggio che arriva all'Exchange portfolio.events
        // con routing goods.received
        await _channel.QueueBindAsync(
            queue: "inventory.goods-received",
            exchange: "portfolio.events",
            routingKey: "goods.received",
            cancellationToken: stoppingToken);

        Console.WriteLine(
            "RabbitMQ GoodsReceived consumer ready.");

        // Consumer che rimane in attesa dei messaggi
        var consumer = new AsyncEventingBasicConsumer(_channel);

        // Quando viene ricevuto un messaggio
        consumer.ReceivedAsync += async (_, ea) =>
        {
            // transazione
            try
            {
                // Il messaggio viene trasformato in JSON
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                // Il JSON viene trasformato in oggetto GoodsReceivedEvent
                var message = JsonSerializer.Deserialize<GoodsReceivedEvent>(json);

                // Errore nel caso di messaggio non valido (oggetto nullo)
                if (message is null)
                {
                    throw new InvalidOperationException(
                        "Invalid GoodsReceived message.");
                }

                // Imposto uno scope a livello dell'intera applicazione
                using var scope = _scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

                // messaggio GoodsReceived 
                var messageType = "GoodsReceived";
                var messageId = message.PurchaseOrderId.ToString();

                // 1. Controlliamo se il messaggio è già stato elaborato
                var alreadyProcessed =
                    await dbContext.ProcessedMessages
                        .AnyAsync(x =>
                                x.MessageType == messageType &&
                                x.MessageId == messageId,
                            stoppingToken);

                // Il messaggio è già stato elaborato
                if (alreadyProcessed)
                {
                    Console.WriteLine(
                        $"GoodsReceived PO {message.PurchaseOrderId} " +
                        "already processed. Skipping.");

                    // Avvisa RabbitMQ che il messaggio è stato gestito correttamente
                    // e può considerarlo completato.
                    await _channel.BasicAckAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        cancellationToken: stoppingToken);

                    return;
                }

                var strategy = dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    // Apre la transazione al database SQL
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                    try
                    {
                        // Aggiorna tutte le giacenze nel database SQL
                        foreach (var line in message.Lines)
                        {
                            await inventoryService.ReceiveAsync(
                                new ReceiveInventoryRequest(
                                    line.ProductId,
                                    line.Quantity,
                                    "PurchaseOrder",
                                    message.PurchaseOrderId),
                                stoppingToken);
                        }

                        // Registra il messaggio come elaborato
                        dbContext.ProcessedMessages.Add(
                            new ProcessedMessage
                            {
                                MessageType = messageType,
                                MessageId = messageId,
                                ProcessedAtUtc = DateTime.UtcNow 
                            });

                        // Salva la transazione
                        await dbContext.SaveChangesAsync(stoppingToken);

                        // Conferma la transazione SQL e diventa definitiva
                        await transaction.CommitAsync(stoppingToken);
                    }
                    catch
                    {
                        // in caso di errore Rollback
                        await transaction.RollbackAsync(
                            stoppingToken);

                        throw;
                    }

                });

                // Dopo il COMMIT invia conferma a RabbitMQ
                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                Console.WriteLine(
                    $"GoodsReceived PO {message.PurchaseOrderId} " +
                    "processed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing GoodsReceived: {ex.Message}");

                // manda un messaggio di fallimento a RabbitQM
                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false, // non rimetterlo nella stessa queue
                    cancellationToken: stoppingToken);
            }
        };

        // Dice a RabbitQM che consumer appena creato
        // deve iniziare ad ascoltare la queue
        await _channel.BasicConsumeAsync(
            queue: "inventory.goods-received",
            autoAck: false, // non configurare il messaggio automaticamente come completato dopo l'invio
            consumer: consumer,
            cancellationToken: stoppingToken);

        // serve a mantenere vivo il BackgroundService
        // altrimenti terminerebbe dopo aver registrato il consumer
        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}