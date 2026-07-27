namespace InventarioService.Domain.Events;

public class VentaRegistradaEvent: IntegrationEvent
{
    public string NumeroVenta { get; set; }
    public List<VentaItem> Items { get; set; }
}

public class VentaItem
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}