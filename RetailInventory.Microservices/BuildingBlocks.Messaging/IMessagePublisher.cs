namespace BuildingBlocks.Messaging.RabbiMQ;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string queue, T message);
}