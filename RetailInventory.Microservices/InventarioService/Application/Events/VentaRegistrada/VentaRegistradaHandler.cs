using BuildingBlocks.Messaging;
using BuildingBlocks.Observability.Services;
using InventarioService.Application.Commands.RegistrarSalida;
using InventarioService.Domain.Events;
using MediatR;
using System.Text;
using System.Text.Json;

namespace InventarioService.Application.Events.VentaRegistrada;

public sealed class VentaRegistradaHandler
{
    private readonly InventarioDbContext _db;
    private readonly IMediator _mediator;
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<VentaRegistradaHandler> _logger;

    public VentaRegistradaHandler(
        InventarioDbContext db,
        IMediator mediator,
        ICorrelationContext correlationContext,
        ILogger<VentaRegistradaHandler> logger)
    {
        _db = db;
        _mediator = mediator;
        _correlationContext = correlationContext;
        _logger = logger;
    }

    public async Task HandleAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        var message = Encoding.UTF8.GetString(context.Body);

        var evt =
            JsonSerializer.Deserialize<VentaRegistradaEvent>(
                message);

        if (evt is null)
        {
            throw new InvalidOperationException(
                "No se pudo deserializar VentaRegistradaEvent.");
        }

        _correlationContext.SetCorrelationId(
            evt.TraceId);

        if (await ExisteEventoAsync(
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
            await _db.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            foreach (var item in evt.Items)
            {
                var result =
                    await _mediator.Send(
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
                evt);

            await _db.SaveChangesAsync(
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

    private Task<bool> ExisteEventoAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        return _db.EventosProcesados
            .AnyAsync(
                x => x.EventoId == eventId,
                cancellationToken);
    }

    private void GuardarEvento(
        VentaRegistradaEvent evt)
    {
        _db.EventosProcesados.Add(
            new Domain.Entities.EventoProcesado
            {
                EventoId = evt.EventId,
                NombreEvento = nameof(VentaRegistradaEvent),
                ReferenciaNegocio = evt.NumeroVenta,
                Payload = JsonSerializer.Serialize(evt),
                FechaProcesamiento = DateTime.UtcNow
            });
    }
}