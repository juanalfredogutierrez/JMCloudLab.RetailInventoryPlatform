using BuildingBlocks.Domain;

namespace InventarioService.Domain.Entities;

public class EventoProcesado : AuditableEntity
{
    public Guid EventoId { get; set; }
    public string NombreEvento { get; set; }
    public string ReferenciaNegocio { get; set; }
    public string Payload { get; set; }
    public DateTime FechaProcesamiento { get; set; }
   
}