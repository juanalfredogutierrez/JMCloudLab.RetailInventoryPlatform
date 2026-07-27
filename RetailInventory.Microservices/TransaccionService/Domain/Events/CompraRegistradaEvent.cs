namespace TransaccionService.Domain.Events;

public class CompraRegistradaEvent : IntegrationEvent
{
    public string NumeroCompra { get; set; }
    public decimal Total { get; set; }
    public List<CompraRegistradaItemEvent> Items { get; set; }
}

public class CompraRegistradaItemEvent
{
    public int ProductoId { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}