using InventarioService.Domain.Entities;
using MediatR;

namespace InventarioService.Application.Commands.RegistrarEntrada;

public class RegistrarEntradaHandler : IRequestHandler<RegistrarEntradaCommand, bool>
{
    private readonly InventarioDbContext _context;
    private readonly ILogger<RegistrarEntradaHandler> _logger;
    public RegistrarEntradaHandler(InventarioDbContext context, ILogger<RegistrarEntradaHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(RegistrarEntradaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogBusiness($"Registrando entrada para producto {request.ProductoId}");
        var stock = await _context.Existencias
                         .FirstOrDefaultAsync(x => x.ProductoId == request.ProductoId, cancellationToken);

        if (stock == null)
        {
            stock = new ExistenciaProducto
            {
                ProductoId = request.ProductoId,
                CantidadDisponible = request.Cantidad,
                FechaActualizacion = DateTime.Now
            };

            _context.Existencias.Add(stock);
            _logger.LogBusiness($"Existencia creada para producto {request.ProductoId}");
        }
        else
        {
            stock.CantidadDisponible += request.Cantidad;
            stock.FechaActualizacion = DateTime.Now;
            _logger.LogBusiness( $"Stock incrementado para producto {request.ProductoId}");
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}