using BuildingBlocks.Messaging.RabbitMQ;
using BuildingBlocks.Observability.Services;
using InventarioService.Application.Commands.RegistrarEntrada;
using InventarioService.Application.Commands.RegistrarSalida;
using InventarioService.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InventarioService.Infrastructure.Messaging;

public sealed class RabbitMqConsumerWorker : BackgroundService
{
    private const string CompraRegistradaQueue = "compra.registrada";
    private const string VentaRegistradaQueue = "venta.registrada";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly ILogger<RabbitMqConsumerWorker> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumerWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> rabbitMqOptions,
        ILogger<RabbitMqConsumerWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _rabbitMqOptions = rabbitMqOptions.Value;
        _logger = logger;
    }

    public override async Task StartAsync(
        CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.Host,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };

        _connection = await factory.CreateConnectionAsync(
            cancellationToken);

        _channel = await _connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: CompraRegistradaQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: VentaRegistradaQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "RabbitMQ Consumer iniciado. Queues: {CompraQueue}, {VentaQueue}",
            CompraRegistradaQueue,
            VentaRegistradaQueue);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException(
                "RabbitMQ channel no inicializado.");
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                await ProcesarMensaje(
                    ea,
                    stoppingToken);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "Mensaje procesado y confirmado. " +
                    "RoutingKey: {RoutingKey} | DeliveryTag: {DeliveryTag}",
                    ea.RoutingKey,
                    ea.DeliveryTag);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Procesamiento cancelado durante el cierre del consumidor. " +
                    "RoutingKey: {RoutingKey} | DeliveryTag: {DeliveryTag}",
                    ea.RoutingKey,
                    ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error procesando mensaje RabbitMQ. " +
                    "RoutingKey: {RoutingKey} | DeliveryTag: {DeliveryTag}",
                    ea.RoutingKey,
                    ea.DeliveryTag);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await _channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: CancellationToken.None);

                    _logger.LogWarning(
                        "Mensaje enviado nuevamente a la cola para reintento. " +
                        "RoutingKey: {RoutingKey} | DeliveryTag: {DeliveryTag}",
                        ea.RoutingKey,
                        ea.DeliveryTag);
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: CompraRegistradaQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await _channel.BasicConsumeAsync(
            queue: VentaRegistradaQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Cierre normal del BackgroundService.
        }
    }

    private async Task ProcesarMensaje(
        BasicDeliverEventArgs ea,
        CancellationToken cancellationToken)
    {
        var message = Encoding.UTF8.GetString(
            ea.Body.ToArray());

        await using var scope =
            _scopeFactory.CreateAsyncScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<InventarioDbContext>();

        var mediator =
            scope.ServiceProvider
                .GetRequiredService<IMediator>();

        var correlationContext =
            scope.ServiceProvider
                .GetRequiredService<ICorrelationContext>();

        switch (ea.RoutingKey)
        {
            case CompraRegistradaQueue:

                await ProcesarCompra(
                    message,
                    db,
                    mediator,
                    correlationContext,
                    cancellationToken);

                break;

            case VentaRegistradaQueue:

                await ProcesarVenta(
                    message,
                    db,
                    mediator,
                    correlationContext,
                    cancellationToken);

                break;

            default:

                throw new InvalidOperationException(
                    $"RoutingKey no soportado: {ea.RoutingKey}");
        }
    }

    private async Task ProcesarCompra(
        string message,
        InventarioDbContext db,
        IMediator mediator,
        ICorrelationContext correlationContext,
        CancellationToken cancellationToken)
    {
        var evt =
            JsonSerializer.Deserialize<CompraRegistradaEvent>(
                message);

        if (evt is null)
        {
            throw new InvalidOperationException(
                "No se pudo deserializar CompraRegistradaEvent.");
        }

        correlationContext.SetCorrelationId(
            evt.TraceId);

        if (await ExisteEvento(
                db,
                evt.EventId,
                cancellationToken))
        {
            _logger.LogInformation(
                "Evento {EventId} ya procesado. " +
                "Se evita el reprocesamiento.",
                evt.EventId);

            return;
        }

        await using var transaction =
            await db.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            foreach (var item in evt.Items)
            {
                var result =
                    await mediator.Send(
                        new RegistrarEntradaCommand(
                            item.ProductoId,
                            item.Cantidad,
                            evt.NumeroCompra),
                        cancellationToken);

                if (!result)
                {
                    throw new InvalidOperationException(
                        $"No se pudo registrar entrada " +
                        $"para el producto {item.ProductoId}.");
                }
            }

            GuardarEvento(
                db,
                evt.EventId,
                nameof(CompraRegistradaEvent),
                evt.NumeroCompra,
                JsonSerializer.Serialize(evt));

            await db.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Compra {NumeroCompra} procesada correctamente. " +
                "EventoId: {EventId}",
                evt.NumeroCompra,
                evt.EventId);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _logger.LogWarning(
                "Transacción revertida para la compra {NumeroCompra}. " +
                "EventoId: {EventId}",
                evt.NumeroCompra,
                evt.EventId);

            throw;
        }
    }

    private async Task ProcesarVenta(
        string message,
        InventarioDbContext db,
        IMediator mediator,
        ICorrelationContext correlationContext,
        CancellationToken cancellationToken)
    {
        var evt =
            JsonSerializer.Deserialize<VentaRegistradaEvent>(
                message);

        if (evt is null)
        {
            throw new InvalidOperationException(
                "No se pudo deserializar VentaRegistradaEvent.");
        }

        correlationContext.SetCorrelationId(
            evt.TraceId);

        if (await ExisteEvento(
                db,
                evt.EventId,
                cancellationToken))
        {
            _logger.LogInformation(
                "Evento {EventId} ya procesado. " +
                "Se evita el reprocesamiento.",
                evt.EventId);

            return;
        }

        await using var transaction =
            await db.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            foreach (var item in evt.Items)
            {
                var result =
                    await mediator.Send(
                        new RegistrarSalidaCommand(
                            item.ProductoId,
                            item.Cantidad),
                        cancellationToken);

                if (result.IsFailure)
                {
                    throw new InvalidOperationException(
                        $"No se pudo registrar salida " +
                        $"para el producto {item.ProductoId}.");
                }
            }

            GuardarEvento(
                db,
                evt.EventId,
                nameof(VentaRegistradaEvent),
                evt.NumeroVenta,
                JsonSerializer.Serialize(evt));

            await db.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Venta {NumeroVenta} procesada correctamente. " +
                "EventoId: {EventId}",
                evt.NumeroVenta,
                evt.EventId);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _logger.LogWarning(
                "Transacción revertida para la venta {NumeroVenta}. " +
                "EventoId: {EventId}",
                evt.NumeroVenta,
                evt.EventId);

            throw;
        }
    }

    private static Task<bool> ExisteEvento(
        InventarioDbContext db,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        return db.EventosProcesados
            .AnyAsync(
                x => x.EventoId == eventId,
                cancellationToken);
    }

    private static void GuardarEvento(
        InventarioDbContext db,
        Guid eventId,
        string nombreEvento,
        string referenciaNegocio,
        string payload)
    {
        db.EventosProcesados.Add(
            new Domain.Entities.EventoProcesado
            {
                EventoId = eventId,
                NombreEvento = nombreEvento,
                ReferenciaNegocio = referenciaNegocio,
                Payload = payload,
                FechaProcesamiento = DateTime.UtcNow
            });
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "RabbitMQ Consumer detenido.");

        await base.StopAsync(
            cancellationToken);

        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}