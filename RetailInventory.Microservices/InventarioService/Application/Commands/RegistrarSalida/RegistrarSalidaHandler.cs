using InventarioService.Domain.Errors;
using MediatR;

namespace InventarioService.Application.Commands.RegistrarSalida;

public class RegistrarSalidaHandler
    : IRequestHandler<RegistrarSalidaCommand, Result>
{
    private readonly InventarioDbContext _context;
    private readonly ILogger<RegistrarSalidaHandler> _logger;

    public RegistrarSalidaHandler(
        InventarioDbContext context,
        ILogger<RegistrarSalidaHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(
        RegistrarSalidaCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogBusiness(
            $"Registrando salida para producto {request.ProductoId}");

        var stock = await _context.Existencias.FirstOrDefaultAsync(
                x => x.ProductoId == request.ProductoId,
                cancellationToken);

        if (stock is null)
        {
            _logger.LogWarning(
                "No existe inventario para producto {ProductoId}",
                request.ProductoId);

            return Result.Failure(
                InventarioErrors.ProductoNoEncontrado);
        }

        if (stock.CantidadDisponible < request.Cantidad)
        {
            _logger.LogWarning(
                "Stock insuficiente para producto {ProductoId}. Disponible: {Disponible}, Solicitado: {Solicitado}",
                request.ProductoId,
                stock.CantidadDisponible,
                request.Cantidad);

            return Result.Failure(
                InventarioErrors.StockInsuficiente);
        }

        stock.CantidadDisponible -= request.Cantidad;
        stock.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogBusiness(
            $"Stock descontado para producto {request.ProductoId}");

        return Result.Success();
    }
}