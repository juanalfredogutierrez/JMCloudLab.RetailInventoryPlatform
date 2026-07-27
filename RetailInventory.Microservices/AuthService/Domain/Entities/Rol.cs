using BuildingBlocks.Domain;

namespace AuthService.Domain.Entities;

public class Rol : AuditableEntity
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }

    public List<Usuario> Usuarios { get; set; } = new();
}