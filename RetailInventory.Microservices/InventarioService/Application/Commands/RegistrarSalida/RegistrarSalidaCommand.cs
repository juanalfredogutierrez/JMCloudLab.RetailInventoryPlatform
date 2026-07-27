using BuildingBlocks.Application;
using MediatR;

namespace InventarioService.Application.Commands.RegistrarSalida;

public record RegistrarSalidaCommand(int ProductoId,int Cantidad) : IRequest<Result>;
