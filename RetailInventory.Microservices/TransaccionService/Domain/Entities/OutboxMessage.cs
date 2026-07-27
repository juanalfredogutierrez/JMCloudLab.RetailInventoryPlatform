using BuildingBlocks.Domain;
using System.Text.Json;

namespace TransaccionService.Domain.Entities;

public sealed class OutboxMessage : AuditableEntity
{
    public string EventType { get; private set; }

    public string Payload { get; private set; }

    public DateTime OccurredOn { get; private set; }

    public DateTime? ProcessedOn { get; private set; }

    public string Error { get; private set; }

    private OutboxMessage()
    {
    }

    public OutboxMessage(string eventType,string payload)
    {
        EventType = eventType;
        Payload = payload;
        OccurredOn = DateTime.UtcNow;
    }

    public void MarkAsProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
    }
    public static OutboxMessage Create(string eventType,object payload)
    {
        return new OutboxMessage(eventType,JsonSerializer.Serialize(payload));
    }
}