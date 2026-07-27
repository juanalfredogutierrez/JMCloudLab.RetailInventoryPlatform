namespace ProductoService.Application.Commands.CreateProducto;

public record CreateProductoCommand(
    string Codigo,
    string Nombre,
    string Descripcion,
    decimal Precio
) : IRequest<Result<Guid>>;