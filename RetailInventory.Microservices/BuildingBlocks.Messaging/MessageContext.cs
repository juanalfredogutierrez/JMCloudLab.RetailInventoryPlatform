namespace BuildingBlocks.Messaging;

public sealed record MessageContext(
    string MessageId,
    string MessageType,
    byte[] Body,
    IReadOnlyDictionary<string, string> Headers = null,
    string CorrelationId = null);