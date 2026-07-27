using BuildingBlocks.Domain;

namespace TransaccionService.Domain.Entities;

public class DetalleVenta : Entity
{
    public int VentaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}