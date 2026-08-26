using BuildingBlocks.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly SemaphoreSlim _publishLock = new(1, 1);

    private IConnection _connection;
    private IChannel _channel;

    public RabbitMqPublisher(
        RabbitMqOptions options,
        ILogger<RabbitMqPublisher> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task PublishAsync<T>(
        string messageType,
        T message,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
        ArgumentNullException.ThrowIfNull(message);

        await _publishLock.WaitAsync(cancellationToken);

        try
        {
            await EnsureConnectionAsync(cancellationToken);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
                MessageId = TryGetEventId(message),
                Headers = BuildHeaders(message)
            };

            await _channel!.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: messageType,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Mensaje confirmado por RabbitMQ. " +
                "MessageType: {MessageType} | MessageId: {MessageId}",
                messageType,
                properties.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error publicando mensaje en RabbitMQ. " +
                "MessageType: {MessageType}",
                messageType);

            throw;
        }
        finally
        {
            _publishLock.Release();
        }
    }

    private async Task EnsureConnectionAsync(
        CancellationToken cancellationToken)
    {
        if (_connection?.IsOpen == true &&
            _channel?.IsOpen == true)
        {
            return;
        }

        await DisposeConnectionAsync();

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

        _channel = await _connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true),
            cancellationToken);

        _logger.LogInformation(
            "Conexión RabbitMQ Publisher establecida. " +
            "Host: {Host}, Port: {Port}",
            _options.Host,
            _options.Port);
    }

    private static string TryGetEventId<T>(T message)
    {   
        return message is IntegrationEvent integrationEvent
            ? integrationEvent.EventId.ToString()
            : null;
    }

    private static Dictionary<string, object> BuildHeaders<T>(
        T message)
    {
        if (message is not IntegrationEvent integrationEvent ||
            string.IsNullOrWhiteSpace(integrationEvent.TraceId))
        {
            return null;
        }

        return new Dictionary<string, object>
        {
            ["trace-id"] = integrationEvent.TraceId
        };
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
                    "Error liberando RabbitMQ channel.");
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
                    "Error liberando RabbitMQ connection.");
            }

            _connection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _publishLock.Dispose();

        await DisposeConnectionAsync();
    }
}