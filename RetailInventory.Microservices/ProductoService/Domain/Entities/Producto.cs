using BuildingBlocks.Domain;

namespace ProductoService.Domain.Entities;

public class Producto : AuditableEntity
{
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
}