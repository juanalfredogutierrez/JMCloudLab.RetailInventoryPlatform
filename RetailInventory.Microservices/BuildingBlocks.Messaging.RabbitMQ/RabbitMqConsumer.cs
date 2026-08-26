using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqConsumer : IMessageConsumer, IAsyncDisposable
{
    private const string CompraRegistradaQueue = "compra.registrada";
    private const string VentaRegistradaQueue = "venta.registrada";

    private const int InitialRetryDelaySeconds = 5;
    private const int MaxRetryDelaySeconds = 30;

    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private IConnection _connection;
    private IChannel _channel;

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

        var retryDelay = InitialRetryDelaySeconds;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ConnectAsync(
                    cancellationToken);

                await StartConsumersAsync(
                    handler,
                    cancellationToken);

                _logger.LogInformation(
                    "RabbitMQ Consumer iniciado correctamente.");

                return;
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "No fue posible establecer el consumidor RabbitMQ. " +
                    "Nuevo intento en {RetryDelaySeconds} segundos.",
                    retryDelay);

                await DisposeConnectionAsync();

                await Task.Delay(
                    TimeSpan.FromSeconds(retryDelay),
                    cancellationToken);

                retryDelay = Math.Min(
                    retryDelay * 2,
                    MaxRetryDelaySeconds);
            }
        }
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
            TopologyRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = await factory.CreateConnectionAsync(
            cancellationToken);

        _connection.ConnectionShutdownAsync +=
            OnConnectionShutdownAsync;

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
                var headers =
                    ExtractHeaders(
                        ea.BasicProperties?.Headers);

                var messageId =
                    ea.BasicProperties?.MessageId
                    ?? ea.DeliveryTag.ToString();

                var correlationId =
                    ExtractCorrelationId(
                        ea.BasicProperties?.Headers);

                var context = new MessageContext(
                    MessageId: messageId,
                    MessageType: ea.RoutingKey,
                    Body: ea.Body.ToArray(),
                    Headers: headers,
                    CorrelationId: correlationId);

                await handler(
                    context,
                    cancellationToken);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Mensaje procesado y confirmado. " +
                    "MessageType: {MessageType} | MessageId: {MessageId} | DeliveryTag: {DeliveryTag}",
                    context.MessageType,
                    context.MessageId,
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
                        "Mensaje reenviado a la cola para reintento. " +
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
            "Consumers registrados. " +
            "Queues: {CompraQueue}, {VentaQueue}",
            CompraRegistradaQueue,
            VentaRegistradaQueue);
    }   
    private Task OnConnectionShutdownAsync(
        object sender,
        ShutdownEventArgs args)
    {
        _logger.LogWarning(
            "Conexión RabbitMQ cerrada. " +
            "Code: {ReplyCode}, Text: {ReplyText}",
            args.ReplyCode,
            args.ReplyText);

        return Task.CompletedTask;
    }

    private async Task DisposeConnectionAsync()
    {
        if (_channel is not null)
        {
            try
            {
                await _channel.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(
                    ex,
                    "Error al liberar RabbitMQ channel.");
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
                _logger.LogDebug(
                    ex,
                    "Error al liberar RabbitMQ connection.");
            }

            _connection = null;
        }
    }

    private static IReadOnlyDictionary<string, string>? ExtractHeaders(
    IDictionary<string, object> headers)
    {
        if (headers is null || headers.Count == 0)
            return null;

        var result = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var header in headers)
        {
            if (header.Value is byte[] bytes)
            {
                result[header.Key] =
                    Encoding.UTF8.GetString(bytes);
            }
            else if (header.Value is not null)
            {
                result[header.Key] =
                    header.Value.ToString() ?? string.Empty;
            }
        }

        return result;
    }

    private static string ExtractCorrelationId(
        IDictionary<string, object> headers)
    {
        if (headers is null ||
            !headers.TryGetValue("trace-id", out var value) ||
            value is null)
        {
            return null;
        }

        return value is byte[] bytes
            ? Encoding.UTF8.GetString(bytes)
            : value.ToString();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeConnectionAsync();
    }



}