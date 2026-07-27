using BuildingBlocks.Domain;

namespace InventarioService.Domain.Entities;

public class MovimientoInventario : AuditableEntity
{
    public string NumeroMovimiento { get; set; }
    public string TipoMovimiento { get; set; } // ENTRADA / SALIDA
    public DateTime FechaMovimiento { get; set; }
    public string OrigenMovimiento { get; set; }
    public string ReferenciaOrigen { get; set; }
    public string Observacion { get; set; }

    public List<DetalleMovimientoInventario> Detalles { get; set; } = new();
}