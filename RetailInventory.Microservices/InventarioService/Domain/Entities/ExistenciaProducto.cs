using BuildingBlocks.Domain;

namespace InventarioService.Domain.Entities;

public class ExistenciaProducto : AuditableEntity
{
    public int ProductoId { get; set; }
    public int CantidadDisponible { get; set; }
    public DateTime FechaActualizacion { get; set; }
}