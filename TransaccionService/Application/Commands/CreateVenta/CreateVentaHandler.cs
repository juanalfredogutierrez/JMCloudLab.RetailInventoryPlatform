using BuildingBlocks.Observability.Services;
using System.Text.Json;
using TransaccionService.Application.DTOs;
using TransaccionService.Domain.Entities;
using TransaccionService.Domain.Errors;
using TransaccionService.Domain.Events;

namespace TransaccionService.Application.Commands.CreateVenta;

public class CreateVentaHandler : IRequestHandler<CreateVentaCommand, Result<Guid>>
{
    private readonly ILogger<CreateVentaHandler> _logger;
    private readonly TransaccionDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICorrelationContext _correlationContext;

    public CreateVentaHandler(
        TransaccionDbContext context,
        ILogger<CreateVentaHandler> logger,
        IHttpClientFactory httpClientFactory,
        ICorrelationContext correlationContext)
    {
        _context = context;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _correlationContext = correlationContext;
    }

    public async Task<Result<Guid>> Handle(CreateVentaCommand request,CancellationToken cancellationToken)
    {
        _logger.LogBusiness($"Registrando venta con {request.Detalles.Count} productos");

        var validacionesStock = request.Detalles.Select(async detalle =>
        {
            _logger.LogBusiness($"Consultando stock del producto {detalle.ProductoId}");

            var stockDisponible = await ObtenerStockAsync(detalle.ProductoId);

            if (stockDisponible < detalle.Cantidad)
            {
                _logger.LogWarning(
                    "Stock insuficiente para producto {ProductoId}. Disponible: {Disponible}, Solicitado: {Solicitado}",
                    detalle.ProductoId,
                    stockDisponible,
                    detalle.Cantidad);

                return VentaErrors.StockInsuficiente(
                    detalle.ProductoId,
                    stockDisponible,
                    detalle.Cantidad);
            }

            return null;
        });

        var errores = await Task.WhenAll(validacionesStock);

        var erroresStock = errores
            .Where(x => x is not null)
            .Cast<Error>()
            .ToArray();

        if (erroresStock.Any())
        {
            return Result<Guid>.Failure(
                erroresStock);
        }

        var venta = new Venta
        {
            NumeroVenta = $"VEN-{Guid.NewGuid().ToString()[..6]}",
            FechaVenta = DateTime.UtcNow,
            Estado = "CREADA",
            Observacion = request.Observacion,
            TotalVenta = request.Detalles.Sum(
                x => x.Cantidad * x.PrecioUnitario),

            Detalles = request.Detalles
                .Select(x => new DetalleVenta
                {
                    ProductoId = x.ProductoId,
                    Cantidad = x.Cantidad,
                    PrecioUnitario = x.PrecioUnitario,
                    Subtotal = x.Cantidad * x.PrecioUnitario
                })
                .ToList()
        };

        _context.Ventas.Add(venta);

        _logger.LogInformation("Venta {NumeroVenta} registrada", venta.NumeroVenta);

        var integrationEvent = new VentaRegistradaEvent
        {
            NumeroVenta = venta.NumeroVenta,
            TraceId = _correlationContext.CorrelationId,

            Items = venta.Detalles
              .Select(x => new VentaRegistradaItemEvent 
              {
                  ProductoId = x.ProductoId,
                  Cantidad = x.Cantidad
              })
              .ToList()
        };

        _context.OutboxMessages.Add(new OutboxMessage("venta.registrada",
                                        JsonSerializer.Serialize(integrationEvent)));

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Evento venta.registrada agregado a Outbox");

        return Result<Guid>.Success(venta.Uid);
    }

    private async Task<int> ObtenerStockAsync(int productoId)
    {
        var client = _httpClientFactory.CreateClient("InventarioApi");

        var result = await client.GetFromJsonAsync<ApiResponse<int>>(
            $"api/inventario/stock/{productoId}");

        return result?.Data ?? 0;
    }
}