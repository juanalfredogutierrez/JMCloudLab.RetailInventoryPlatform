namespace BuildingBlocks.Messaging;

public sealed record MessageContext(
    string RoutingKey,
    ulong DeliveryTag,
    byte[] Body);