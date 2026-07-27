namespace TransaccionService.Application.Commands.CreateCompra;

public record CreateCompraCommand(List<DetalleCompraDto> Detalles, string Observacion) : IRequest<Result<Guid>>;
public record DetalleCompraDto(int ProductoId, int Cantidad, decimal PrecioUnitario);