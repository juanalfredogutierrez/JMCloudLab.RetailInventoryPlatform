using BuildingBlocks.Application;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductoService.Infrastructure.Persistence;

namespace ProductoService.Application.Commands.UpdateProduct
{
    public class ActualizarCostoProductoHandler
      : IRequestHandler<ActualizarCostoProductoCommand, Result<Guid>>
    {
        private readonly ProductoDbContext _context;

        public ActualizarCostoProductoHandler(
            ProductoDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(ActualizarCostoProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _context.Productos
                             .FirstOrDefaultAsync(x => x.Id == request.ProductoId, cancellationToken);

            if (producto is null)
            {
                return Result<Guid>.Failure(
                    new Error(
                        "Producto.NoExiste",
                        "Producto no encontrado"));
            }

            producto.Precio =
                request.Costo;

            producto.Precio =
                Math.Round(
                    request.Costo * 1.35m,
                    2);

            await _context.SaveChangesAsync(
                cancellationToken);

            return Result<Guid>.Success(producto.Uid);
        }
    }
}
