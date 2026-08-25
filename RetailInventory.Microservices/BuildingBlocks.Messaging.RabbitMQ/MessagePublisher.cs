using BuildingBlocks.Messaging;
using BuildingBlocks.Observability.Constants;
using BuildingBlocks.Observability.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<RabbitMqPublisher> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqPublisher(
        RabbitMqOptions options,
        ICorrelationContext correlationContext,
        ILogger<RabbitMqPublisher> logger)
    {
        _options = options;
        _correlationContext = correlationContext;
        _logger = logger;
    }

    public async Task PublishAsync<T>(
        string queue,
        T message)
    {
        await EnsureConnectionAsync();

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object>
            {
                [HeaderNames.CorrelationId] =
                    _correlationContext.CorrelationId
            }
        };

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        await _channel!.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogInformation(
            "Mensaje publicado en RabbitMQ. RoutingKey: {RoutingKey}",
            queue);
    }

    private async Task EnsureConnectionAsync()
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
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            TopologyRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync();

        _channel = await _connection.CreateChannelAsync();

        _logger.LogInformation(
            "Conexión RabbitMQ Publisher establecida. " +
            "Host: {Host}, Port: {Port}",
            _options.Host,
            _options.Port);
    }

    private async Task DisposeConnectionAsync()
    {
        if (_channel is not null)
        {
            try
            {
                await _channel.DisposeAsync();
            }
            catch
            {
                // La conexión puede estar ya cerrada.
            }

            _channel = null;
        }

        if (_connection is not null)
        {
            try
            {
                await _connection.DisposeAsync();
            }
            catch
            {
                // La conexión puede estar ya cerrada.
            }

            _connection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeConnectionAsync();
    }
}