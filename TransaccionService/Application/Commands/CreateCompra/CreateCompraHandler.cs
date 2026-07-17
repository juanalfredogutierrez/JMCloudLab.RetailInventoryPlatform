using System.Text.Json;
using TransaccionService.Domain.Entities;
using TransaccionService.Domain.Events;
using BuildingBlocks.Observability.Services;
namespace TransaccionService.Application.Commands.CreateCompra;

public class CreateCompraHandler : IRequestHandler<CreateCompraCommand, Result<Guid>>
{
    private readonly TransaccionDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CreateCompraHandler> _logger;
    private readonly ICorrelationContext _correlationContext;
    public CreateCompraHandler(
        TransaccionDbContext context,
        ILogger<CreateCompraHandler> logger,
        IHttpClientFactory httpClientFactory,
        ICorrelationContext correlationContext)
    {
        _context = context;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _correlationContext = correlationContext;
    }

    public async Task<Result<Guid>> Handle( CreateCompraCommand request,CancellationToken cancellationToken)
    {
        _logger.LogBusiness($"Registrando compra con {request.Detalles.Count} productos");

        var compra = new Compra
        {
            NumeroCompra =$"COM-{Guid.NewGuid().ToString()[..6]}",
            FechaCompra = DateTime.UtcNow,
            Estado = "CREADA",
            Observacion = request.Observacion,
            TotalCompra = request.Detalles.Sum(x => x.Cantidad * x.PrecioUnitario),

            Detalles = request.Detalles
                .Select(x => new DetalleCompra
                {
                    ProductoId = x.ProductoId,
                    Cantidad = x.Cantidad,
                    PrecioUnitario = x.PrecioUnitario,
                    Subtotal = x.Cantidad * x.PrecioUnitario
                })
                .ToList()
        };

        _context.Compras.Add(compra);

        var productoClient =_httpClientFactory.CreateClient("ProductoApi");

        foreach (var item in compra.Detalles)
        {
            var response =
                await productoClient.PutAsJsonAsync(
                    "api/producto/actualizar-costo",
                    new
                    {
                        ProductoId = item.ProductoId,
                        Costo = item.PrecioUnitario
                    },
                    cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        _logger.LogInformation("Compra {NumeroCompra} registrada",compra.NumeroCompra);

        var integrationEvent = new CompraRegistradaEvent
        {
            NumeroCompra = compra.NumeroCompra,
            Total = compra.TotalCompra,
            TraceId = _correlationContext.CorrelationId,

            Items = compra.Detalles
                .Select(x => new CompraRegistradaItemEvent
                {
                    ProductoId = x.ProductoId,
                    Cantidad = x.Cantidad,
                    PrecioUnitario = x.PrecioUnitario
                })
                .ToList()
        };

        _context.OutboxMessages.Add( new OutboxMessage("compra.registrada",
                                        JsonSerializer.Serialize(integrationEvent)));

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Evento compra.registrada agregado a Outbox");


        return Result<Guid>.Success(compra.Uid);
    }
}