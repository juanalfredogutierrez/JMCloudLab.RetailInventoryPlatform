using BuildingBlocks.Domain;

namespace TransaccionService.Domain.Entities;

public class Compra : AuditableEntity
{
    public string NumeroCompra { get; set; }
    public DateTime FechaCompra { get; set; }
    public decimal TotalCompra { get; set; }
    public string Estado { get; set; }
    public string Observacion { get; set; }

    public List<DetalleCompra> Detalles { get; set; } = new();
}