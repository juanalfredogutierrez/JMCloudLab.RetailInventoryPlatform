using BuildingBlocks.Domain;

namespace TransaccionService.Domain.Entities;

public class Venta : AuditableEntity
{
    public string NumeroVenta { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal TotalVenta { get; set; }
    public string Estado { get; set; }
    public string Observacion { get; set; }

    public List<DetalleVenta> Detalles { get; set; } = new();
}