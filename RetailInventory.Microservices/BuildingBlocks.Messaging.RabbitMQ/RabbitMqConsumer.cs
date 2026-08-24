using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqConsumer : IMessageConsumer, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumer(RabbitMqOptions options)
    {
        _options = options;
    }

    public async Task StartAsync(
        Func<MessageContext, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);

        _channel = await _connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: "compra.registrada",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: "venta.registrada",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var context = new MessageContext(
                    ea.RoutingKey,
                    ea.DeliveryTag,
                    ea.Body.ToArray());

                await handler(context, cancellationToken);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                // Cierre controlado.
            }
            catch
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    await _channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: CancellationToken.None);
                }

                throw;
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "compra.registrada",
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        await _channel.BasicConsumeAsync(
            queue: "venta.registrada",
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}