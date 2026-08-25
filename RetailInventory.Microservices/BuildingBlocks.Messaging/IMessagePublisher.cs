namespace BuildingBlocks.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(
        string queue,
        T message);
}