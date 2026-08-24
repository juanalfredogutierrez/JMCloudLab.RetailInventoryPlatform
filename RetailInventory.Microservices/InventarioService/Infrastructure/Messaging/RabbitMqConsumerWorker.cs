using BuildingBlocks.Messaging;
using BuildingBlocks.Observability.Services;
using InventarioService.Application.Commands.RegistrarEntrada;
using InventarioService.Application.Commands.RegistrarSalida;
using InventarioService.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace InventarioService.Infrastructure.Messaging;

public sealed class RabbitMqConsumerWorker : BackgroundService
{
    private const string CompraRegistradaQueue = "compra.registrada";
    private const string VentaRegistradaQueue = "venta.registrada";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageConsumer _messageConsumer;
    private readonly ILogger<RabbitMqConsumerWorker> _logger;

    public RabbitMqConsumerWorker(
        IServiceScopeFactory scopeFactory,
        IMessageConsumer messageConsumer,
        ILogger<RabbitMqConsumerWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _messageConsumer = messageConsumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "RabbitMQ Consumer Worker iniciado.");

        await _messageConsumer.StartAsync(
            ProcesarMensaje,
            stoppingToken);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "RabbitMQ Consumer Worker detenido.");
        }
    }

    private async Task ProcesarMensaje(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        var message = Encoding.UTF8.GetString(context.Body);

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

        switch (context.RoutingKey)
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
                    $"RoutingKey no soportado: {context.RoutingKey}");
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
}