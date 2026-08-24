namespace BuildingBlocks.Messaging;

public interface IMessageConsumer
{
    Task StartAsync(
        Func<MessageContext, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}