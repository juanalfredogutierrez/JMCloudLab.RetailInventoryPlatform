namespace TransaccionService.Application.Commands.CreateVenta;

public record CreateVentaCommand(List<DetalleVentaDto> Detalles, string Observacion) : IRequest<Result<Guid>>;
public record DetalleVentaDto(int ProductoId, int Cantidad, decimal PrecioUnitario);