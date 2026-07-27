using MediatR;

namespace InventarioService.Application.Commands.RegistrarEntrada;

public record RegistrarEntradaCommand(
    int ProductoId,
    int Cantidad,
    string Referencia
) : IRequest<bool>;