using BuildingBlocks.Domain;

namespace InventarioService.Domain.Entities;

public class DetalleMovimientoInventario : Entity
{
    public int MovimientoInventarioId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}