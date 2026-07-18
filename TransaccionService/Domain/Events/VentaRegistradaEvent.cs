namespace TransaccionService.Domain.Events;

public class VentaRegistradaEvent: IntegrationEvent
{
    public string NumeroVenta { get; set; } = string.Empty;

    public List<VentaRegistradaItemEvent> Items { get; set; } = [];
}

public class VentaRegistradaItemEvent
{
    public int ProductoId { get; set; }

    public int Cantidad { get; set; }
}