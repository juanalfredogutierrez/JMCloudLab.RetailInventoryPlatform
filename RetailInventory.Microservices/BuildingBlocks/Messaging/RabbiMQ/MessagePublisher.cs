using BuildingBlocks.Observability.Constants;
using BuildingBlocks.Observability.Services;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.Messaging.RabbiMQ;

public sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly IChannel _channel;
    private readonly ICorrelationContext _correlationContext;

    public RabbitMqPublisher(
        IChannel channel,
        ICorrelationContext correlationContext)
    {
        _channel = channel;
        _correlationContext = correlationContext;
    }

    public async Task PublishAsync<T>( string queue,T message)
    {
        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object>
            {
                [HeaderNames.CorrelationId] = _correlationContext.CorrelationId
            }
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}