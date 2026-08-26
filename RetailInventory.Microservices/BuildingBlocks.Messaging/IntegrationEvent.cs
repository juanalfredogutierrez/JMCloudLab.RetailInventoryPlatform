namespace BuildingBlocks.Messaging;

public abstract class IntegrationEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string TraceId { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
}