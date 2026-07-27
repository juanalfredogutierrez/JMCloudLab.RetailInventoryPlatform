using BuildingBlocks.Application;
using MediatR;

namespace ProductoService.Application.Commands.UpdateProduct
{
    public record ActualizarCostoProductoCommand(
      int ProductoId,
      decimal Costo)
      : IRequest<Result<Guid>>;
}
