using BuildingBlocks.Messaging;
using BuildingBlocks.Observability.Services;
using InventarioService.Application.Commands.RegistrarEntrada;
using InventarioService.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace InventarioService.Application.Events.CompraRegistrada;

public sealed class CompraRegistradaHandler
{
    private readonly InventarioDbContext _db;
    private readonly IMediator _mediator;
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<CompraRegistradaHandler> _logger;

    public CompraRegistradaHandler(
        InventarioDbContext db,
        IMediator mediator,
        ICorrelationContext correlationContext,
        ILogger<CompraRegistradaHandler> logger)
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
            JsonSerializer.Deserialize<CompraRegistradaEvent>(
                message);

        if (evt is null)
        {
            throw new InvalidOperationException(
                "No se pudo deserializar CompraRegistradaEvent.");
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
                evt);

            await _db.SaveChangesAsync(
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
        CompraRegistradaEvent evt)
    {
        _db.EventosProcesados.Add(
            new Domain.Entities.EventoProcesado
            {
                EventoId = evt.EventId,
                NombreEvento = nameof(CompraRegistradaEvent),
                ReferenciaNegocio = evt.NumeroCompra,
                Payload = JsonSerializer.Serialize(evt),
                FechaProcesamiento = DateTime.UtcNow
            });
    }
}