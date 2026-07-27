using BuildingBlocks.Domain;

namespace AuthService.Domain.Entities;

public class Usuario : AuditableEntity
{
    public string NombreUsuario { get; set; }
    public string CorreoElectronico { get; set; }
    public string ClaveHash { get; set; }
    public bool Activo { get; set; }
    public int RolId { get; set; }
    public Rol Rol { get; set; }
}