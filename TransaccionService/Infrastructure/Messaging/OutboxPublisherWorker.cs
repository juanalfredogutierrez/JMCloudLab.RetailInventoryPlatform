using System.Text.Json;
using TransaccionService.Domain.Entities;
using TransaccionService.Domain.Events;

namespace TransaccionService.Infrastructure.Messaging;

public sealed class OutboxPublisherWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherWorker> _logger;

    public OutboxPublisherWorker(IServiceScopeFactory scopeFactory,
                                ILogger<OutboxPublisherWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxPublisherWorker iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcesarMensajes(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensajes Outbox");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcesarMensajes(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TransaccionDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

        var messages = await context.OutboxMessages
                            .Where(x => x.ProcessedOn == null)
                            .OrderBy(x => x.OccurredOn)
                            .Take(50)
                            .ToListAsync(cancellationToken);

        if (!messages.Any())
            return;

        _logger.LogInformation("Se encontraron {Cantidad} mensajes pendientes", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                await PublicarMensaje(message, publisher, cancellationToken);
                message.MarkAsProcessed();

                _logger.LogInformation("Evento {EventType} procesado", message.EventType);
            }
            catch (Exception ex)
            {
                message.MarkAsFailed(ex.ToString());

                _logger.LogError(ex, "Error procesando evento {EventType}",message.EventType);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task PublicarMensaje(OutboxMessage message, IMessagePublisher publisher, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publicando evento {EventType} | MessageId: {MessageId}",
                                 message.EventType, message.Id);

        switch (message.EventType)
        {
            case "compra.registrada":

                var compraEvent = JsonSerializer.Deserialize<CompraRegistradaEvent>(message.Payload);

                if (compraEvent is null)
                {
                    throw new InvalidOperationException(
                        $"Payload inválido para {message.EventType}. MessageId: {message.Id}");
                }

                await publisher.PublishAsync("compra.registrada", compraEvent);
                break;

            case "venta.registrada":

                var ventaEvent = JsonSerializer.Deserialize<VentaRegistradaEvent>(
                        message.Payload);

                if (ventaEvent is null)
                {
                    throw new InvalidOperationException(
                        $"Payload inválido para {message.EventType}. MessageId: {message.Id}");
                }

                await publisher.PublishAsync("venta.registrada", ventaEvent);   
                break;

            default:
                throw new InvalidOperationException($"Tipo de evento no soportado: {message.EventType}");
        }
    }
}