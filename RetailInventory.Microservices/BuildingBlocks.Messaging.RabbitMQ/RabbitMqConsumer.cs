using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqConsumer : IMessageConsumer, IAsyncDisposable
{
    private const string CompraRegistradaQueue = "compra.registrada";
    private const string VentaRegistradaQueue = "venta.registrada";

    private const string CompraRetry5sQueue =
        "compra.registrada.retry.5s";

    private const string CompraRetry15sQueue =
        "compra.registrada.retry.15s";

    private const string CompraRetry30sQueue =
        "compra.registrada.retry.30s";

    private const string CompraDlqQueue =
        "compra.registrada.dlq";

    private const string VentaRetry5sQueue =
        "venta.registrada.retry.5s";

    private const string VentaRetry15sQueue =
        "venta.registrada.retry.15s";

    private const string VentaRetry30sQueue =
        "venta.registrada.retry.30s";

    private const string VentaDlqQueue =
        "venta.registrada.dlq";

    private const string RetryCountHeader =
        "x-retry-count";

    private const int Retry1DelayMilliseconds = 5_000;
    private const int Retry2DelayMilliseconds = 15_000;
    private const int Retry3DelayMilliseconds = 30_000;

    private const int MaxRetryAttempts = 3;

    private const int InitialConnectionRetryDelaySeconds = 5;
    private const int MaxConnectionRetryDelaySeconds = 30;

    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private readonly SemaphoreSlim _ackLock = new(1, 1);
    private readonly SemaphoreSlim _retryPublishLock = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;
    private IChannel? _retryChannel;

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

        var retryDelay =
            InitialConnectionRetryDelaySeconds;

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
                    MaxConnectionRetryDelaySeconds);
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
            NetworkRecoveryInterval =
                TimeSpan.FromSeconds(10)
        };

        _connection =
            await factory.CreateConnectionAsync(
                cancellationToken);

        _connection.ConnectionShutdownAsync +=
            OnConnectionShutdownAsync;

        _channel =
            await _connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        _retryChannel =
            await _connection.CreateChannelAsync(
                new CreateChannelOptions(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true),
                cancellationToken);

        await ConfigureChannelAsync(
            cancellationToken);

        await ConfigureRetryQueuesAsync(
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

    private async Task ConfigureRetryQueuesAsync(
        CancellationToken cancellationToken)
    {
        await DeclareRetryQueueAsync(
            CompraRetry5sQueue,
            CompraRegistradaQueue,
            Retry1DelayMilliseconds,
            cancellationToken);

        await DeclareRetryQueueAsync(
            CompraRetry15sQueue,
            CompraRegistradaQueue,
            Retry2DelayMilliseconds,
            cancellationToken);

        await DeclareRetryQueueAsync(
            CompraRetry30sQueue,
            CompraRegistradaQueue,
            Retry3DelayMilliseconds,
            cancellationToken);

        await DeclareRetryQueueAsync(
            VentaRetry5sQueue,
            VentaRegistradaQueue,
            Retry1DelayMilliseconds,
            cancellationToken);

        await DeclareRetryQueueAsync(
            VentaRetry15sQueue,
            VentaRegistradaQueue,
            Retry2DelayMilliseconds,
            cancellationToken);

        await DeclareRetryQueueAsync(
            VentaRetry30sQueue,
            VentaRegistradaQueue,
            Retry3DelayMilliseconds,
            cancellationToken);

        await _channel!.QueueDeclareAsync(
            queue: CompraDlqQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: VentaDlqQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "RabbitMQ Retry/DLQ configuradas correctamente.");
    }

    private async Task DeclareRetryQueueAsync(
        string retryQueue,
        string originalQueue,
        int ttlMilliseconds,
        CancellationToken cancellationToken)
    {
        var arguments =
            new Dictionary<string, object?>
            {
                ["x-message-ttl"] = ttlMilliseconds,
                ["x-dead-letter-exchange"] = string.Empty,
                ["x-dead-letter-routing-key"] = originalQueue
            };

        await _channel!.QueueDeclareAsync(
            queue: retryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
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
            var context =
                CreateMessageContext(ea);

            try
            {
                await handler(
                    context,
                    cancellationToken);

                await AckAsync(
                    ea.DeliveryTag,
                    cancellationToken);

                _logger.LogInformation(
                    "Mensaje procesado y confirmado. " +
                    "MessageType: {MessageType} | " +
                    "MessageId: {MessageId} | " +
                    "DeliveryTag: {DeliveryTag}",
                    context.MessageType,
                    context.MessageId,
                    ea.DeliveryTag);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Procesamiento cancelado durante el cierre. " +
                    "MessageType: {MessageType} | " +
                    "MessageId: {MessageId} | " +
                    "DeliveryTag: {DeliveryTag}",
                    context.MessageType,
                    context.MessageId,
                    ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                await HandleProcessingFailureAsync(
                    context,
                    ea.DeliveryTag,
                    ex,
                    cancellationToken);
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

    private async Task HandleProcessingFailureAsync(
        MessageContext context,
        ulong deliveryTag,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Error procesando mensaje RabbitMQ. " +
            "MessageType: {MessageType} | " +
            "MessageId: {MessageId} | " +
            "DeliveryTag: {DeliveryTag}",
            context.MessageType,
            context.MessageId,
            deliveryTag);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        try
        {
            var retryCount =
                GetRetryCount(
                    context.Headers);

            if (retryCount < MaxRetryAttempts)
            {
                await PublishRetryAsync(
                    context,
                    retryCount,
                    cancellationToken);

                await AckAsync(
                    deliveryTag,
                    cancellationToken);

                _logger.LogWarning(
                    "Mensaje enviado a retry. " +
                    "MessageType: {MessageType} | " +
                    "MessageId: {MessageId} | " +
                    "Retry: {Retry}/{MaxRetry}",
                    context.MessageType,
                    context.MessageId,
                    retryCount + 1,
                    MaxRetryAttempts);

                return;
            }

            await PublishToDlqAsync(
                context,
                cancellationToken);

            await AckAsync(
                deliveryTag,
                cancellationToken);

            _logger.LogError(
                "Mensaje enviado a DLQ después de {MaxRetry} intentos. " +
                "MessageType: {MessageType} | " +
                "MessageId: {MessageId}",
                MaxRetryAttempts,
                context.MessageType,
                context.MessageId);
        }
        catch (Exception retryException)
        {
            _logger.LogCritical(
                retryException,
                "No fue posible enviar el mensaje a Retry/DLQ. " +
                "El mensaje permanecerá sin ACK. " +
                "MessageType: {MessageType} | " +
                "MessageId: {MessageId} | " +
                "DeliveryTag: {DeliveryTag}",
                context.MessageType,
                context.MessageId,
                deliveryTag);

            /*
             * No hacemos ACK.
             * Si la conexión/canal se recupera o se cierra,
             * RabbitMQ podrá redeliverar el mensaje.
             */
        }
    }

    private async Task AckAsync(
        ulong deliveryTag,
        CancellationToken cancellationToken)
    {
        if (_channel is null || !_channel.IsOpen)
        {
            throw new InvalidOperationException(
                "RabbitMQ consumer channel no disponible.");
        }

        await _ackLock.WaitAsync(
            cancellationToken);

        try
        {
            await _channel.BasicAckAsync(
                deliveryTag: deliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _ackLock.Release();
        }
    }

    private static MessageContext CreateMessageContext(
        BasicDeliverEventArgs ea)
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

        return new MessageContext(
            MessageId: messageId,
            MessageType: ea.RoutingKey,
            Body: ea.Body.ToArray(),
            Headers: headers,
            CorrelationId: correlationId);
    }

    private static IReadOnlyDictionary<string, string>?
        ExtractHeaders(
            IDictionary<string, object>? headers)
    {
        if (headers is null ||
            headers.Count == 0)
        {
            return null;
        }

        var result =
            new Dictionary<string, string>(
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
                    header.Value.ToString()
                    ?? string.Empty;
            }
        }

        return result;
    }

    private static string? ExtractCorrelationId(
        IDictionary<string, object>? headers)
    {
        if (headers is null ||
            !headers.TryGetValue(
                "trace-id",
                out var value) ||
            value is null)
        {
            return null;
        }

        return value is byte[] bytes
            ? Encoding.UTF8.GetString(bytes)
            : value.ToString();
    }

    private static int GetRetryCount(
        IReadOnlyDictionary<string, string>? headers)
    {
        if (headers is null)
        {
            return 0;
        }

        if (!headers.TryGetValue(
                RetryCountHeader,
                out var value))
        {
            return 0;
        }

        return int.TryParse(
            value,
            out var retryCount)
            ? retryCount
            : 0;
    }

    private static string GetRetryQueue(
        string messageType,
        int retryCount)
    {
        return messageType switch
        {
            CompraRegistradaQueue
                when retryCount == 0 =>
                CompraRetry5sQueue,

            CompraRegistradaQueue
                when retryCount == 1 =>
                CompraRetry15sQueue,

            CompraRegistradaQueue
                when retryCount == 2 =>
                CompraRetry30sQueue,

            VentaRegistradaQueue
                when retryCount == 0 =>
                VentaRetry5sQueue,

            VentaRegistradaQueue
                when retryCount == 1 =>
                VentaRetry15sQueue,

            VentaRegistradaQueue
                when retryCount == 2 =>
                VentaRetry30sQueue,

            _ => throw new InvalidOperationException(
                $"No existe una cola de retry para " +
                $"MessageType '{messageType}' " +
                $"y Retry '{retryCount}'.")
        };
    }

    private async Task PublishRetryAsync(
        MessageContext context,
        int retryCount,
        CancellationToken cancellationToken)
    {
        if (_retryChannel is null ||
            !_retryChannel.IsOpen)
        {
            throw new InvalidOperationException(
                "RabbitMQ retry channel no disponible.");
        }

        var retryQueue =
            GetRetryQueue(
                context.MessageType,
                retryCount);

        var headers =
            BuildRetryHeaders(
                context.Headers,
                retryCount + 1);

        var properties =
            new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
                MessageId = context.MessageId,
                Headers = headers.ToDictionary(
                    x => x.Key,
                    x => (object)x.Value)
            };

        await _retryPublishLock.WaitAsync(
            cancellationToken);

        try
        {
            await _retryChannel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: retryQueue,
                mandatory: true,
                basicProperties: properties,
                body: context.Body,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _retryPublishLock.Release();
        }
    }

    private async Task PublishToDlqAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        if (_retryChannel is null ||
            !_retryChannel.IsOpen)
        {
            throw new InvalidOperationException(
                "RabbitMQ DLQ channel no disponible.");
        }

        var dlqQueue =
            context.MessageType switch
            {
                CompraRegistradaQueue =>
                    CompraDlqQueue,

                VentaRegistradaQueue =>
                    VentaDlqQueue,

                _ => throw new InvalidOperationException(
                    $"No existe DLQ para " +
                    $"MessageType '{context.MessageType}'.")
            };

        var headers =
            BuildRetryHeaders(
                context.Headers,
                GetRetryCount(
                    context.Headers));

        var properties =
            new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
                MessageId = context.MessageId,
                Headers = headers.ToDictionary(
                    x => x.Key,
                    x => (object)x.Value)
            };

        await _retryPublishLock.WaitAsync(
            cancellationToken);

        try
        {
            await _retryChannel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: dlqQueue,
                mandatory: true,
                basicProperties: properties,
                body: context.Body,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _retryPublishLock.Release();
        }
    }

    private static Dictionary<string, string>
        BuildRetryHeaders(
            IReadOnlyDictionary<string, string>?
                currentHeaders,
            int retryCount)
    {
        var headers =
            currentHeaders?
                .ToDictionary(
                    x => x.Key,
                    x => x.Value,
                    StringComparer.OrdinalIgnoreCase)
            ?? new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

        headers[RetryCountHeader] =
            retryCount.ToString();

        return headers;
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
        if (_retryChannel is not null)
        {
            try
            {
                await _retryChannel.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(
                    ex,
                    "Error al liberar RabbitMQ retry channel.");
            }

            _retryChannel = null;
        }

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

    public async ValueTask DisposeAsync()
    {
        await DisposeConnectionAsync();

        _ackLock.Dispose();
        _retryPublishLock.Dispose();
    }
}