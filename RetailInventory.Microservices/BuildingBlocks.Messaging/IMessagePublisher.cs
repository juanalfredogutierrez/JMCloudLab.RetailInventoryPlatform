namespace BuildingBlocks.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(
        string messageType,
        T message,
        CancellationToken cancellationToken = default);
}