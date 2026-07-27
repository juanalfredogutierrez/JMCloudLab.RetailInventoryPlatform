namespace ProductoService.Domain.Events
{
    public record ActualizarCostoProductoEvent(int ProductoId, decimal Costo);
}
