using BuildingBlocks.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqConsumer : IMessageConsumer, IAsyncDisposable
{
    private const string CompraRegistradaQueue = "compra.registrada";
    private const string VentaRegistradaQueue = "venta.registrada";

    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumer(
        RabbitMqOptions options,
        ILogger<RabbitMqConsumer> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task StartAsync(
        Func<MessageContext, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);

        await ConnectAsync(cancellationToken);

        await StartConsumersAsync(
            handler,
            cancellationToken);
    }

    private async Task ConnectAsync(
        CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,

            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            TopologyRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync(
            cancellationToken);

        _channel = await _connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        await ConfigureChannelAsync(
            cancellationToken);

        _logger.LogInformation(
            "Conexión RabbitMQ Consumer establecida. " +
            "Host: {Host}, Port: {Port}",
            _options.Host,
            _options.Port);
    }

    private async Task ConfigureChannelAsync(
        CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException(
                "RabbitMQ channel no inicializado.");
        }

        await _channel.QueueDeclareAsync(
            queue: CompraRegistradaQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: VentaRegistradaQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: cancellationToken);
    }

    private async Task StartConsumersAsync(
        Func<MessageContext, CancellationToken, Task> handler,
        CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException(
                "RabbitMQ channel no inicializado.");
        }

        var consumer =
            new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var context = new MessageContext(
                    ea.RoutingKey,
                    ea.DeliveryTag,
                    ea.Body.ToArray());

                await handler(
                    context,
                    cancellationToken);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Mensaje procesado y confirmado. " +
                    "RoutingKey: {RoutingKey} | DeliveryTag: {DeliveryTag}",
                    ea.RoutingKey,
                    ea.DeliveryTag);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Procesamiento cancelado durante el cierre. " +
                    "RoutingKey: {RoutingKey} | DeliveryTag: {DeliveryTag}",
                    ea.RoutingKey,
                    ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error procesando mensaje RabbitMQ. " +
                    "RoutingKey: {RoutingKey} | DeliveryTag: {DeliveryTag}",
                    ea.RoutingKey,
                    ea.DeliveryTag);

                if (!cancellationToken.IsCancellationRequested)
                {
                    await _channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: CancellationToken.None);

                    _logger.LogWarning(
                        "Mensaje enviado nuevamente a la cola. " +
                        "RoutingKey: {RoutingKey} | DeliveryTag: {DeliveryTag}",
                        ea.RoutingKey,
                        ea.DeliveryTag);
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: CompraRegistradaQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        await _channel.BasicConsumeAsync(
            queue: VentaRegistradaQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Consumers registrados. Queues: {CompraQueue}, {VentaQueue}",
            CompraRegistradaQueue,
            VentaRegistradaQueue);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            try
            {
                await _channel.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Error cerrando RabbitMQ channel.");
            }

            _channel = null;
        }

        if (_connection is not null)
        {
            try
            {
                await _connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Error cerrando RabbitMQ connection.");
            }

            _connection = null;
        }
    }
}