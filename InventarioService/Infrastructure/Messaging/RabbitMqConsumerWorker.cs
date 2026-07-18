using InventarioService.Application.Commands.RegistrarEntrada;
using InventarioService.Application.Commands.RegistrarSalida;
using BuildingBlocks.Observability.Services;
using InventarioService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InventarioService.Infrastructure.Messaging;

public sealed class RabbitMqConsumerWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly ILogger<RabbitMqConsumerWorker> _logger;
    private IConnection _connection;
    private IChannel _channel;

    public RabbitMqConsumerWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> rabbitMqOptions,
        ILogger<RabbitMqConsumerWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _rabbitMqOptions = rabbitMqOptions.Value;
        _logger = logger;
    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.Host,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

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


        _logger.LogInformation("RabbitMQ Consumer iniciado. Queues: {Queues}",
                              "compra.registrada, venta.registrada");

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)   
            throw new InvalidOperationException("RabbitMQ channel no inicializado.");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                await ProcesarMensaje(ea);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensaje RabbitMQ. RoutingKey: {RoutingKey}", ea.RoutingKey);

                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "compra.registrada",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await _channel.BasicConsumeAsync(
            queue: "venta.registrada",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcesarMensaje(BasicDeliverEventArgs ea)
    {
        var message = Encoding.UTF8.GetString(ea.Body.ToArray());

        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var correlationContext = scope.ServiceProvider.GetRequiredService<ICorrelationContext>();

        switch (ea.RoutingKey)
        {
            case "compra.registrada":
                await ProcesarCompra(message, db, mediator, correlationContext);
                break;

            case "venta.registrada":
                await ProcesarVenta(message, db, mediator, correlationContext);
                break;
        }
    }

    private async Task ProcesarCompra(string message, InventarioDbContext db, IMediator mediator, ICorrelationContext correlationContext)
    {
        var evt = JsonSerializer.Deserialize<CompraRegistradaEvent>(message);

        if (evt is null)
            return;

        correlationContext.SetCorrelationId(evt.TraceId);

        if (await ExisteEvento(db, evt.EventId))
            return;

        await GuardarEvento(db, evt.EventId, nameof(CompraRegistradaEvent), evt.NumeroCompra,
    JsonSerializer.Serialize(evt));

        foreach (var item in evt.Items)
        {
            await mediator.Send(new RegistrarEntradaCommand(
                item.ProductoId,
                item.Cantidad,
                evt.NumeroCompra));
        }
    }

    private async Task ProcesarVenta(string message, InventarioDbContext db, IMediator mediator, ICorrelationContext correlationContext)
    {
        var evt = JsonSerializer.Deserialize<VentaRegistradaEvent>(message);

        if (evt is null)
            return;

        correlationContext.SetCorrelationId(evt.TraceId);

        if (await ExisteEvento(db, evt.EventId))
            return;

        await GuardarEvento(db, evt.EventId, nameof(VentaRegistradaEvent), evt.NumeroVenta,
    JsonSerializer.Serialize(evt));

        foreach (var item in evt.Items)
        {
            await mediator.Send(new RegistrarSalidaCommand(item.ProductoId, item.Cantidad));
        }
    }

    private static Task<bool> ExisteEvento(InventarioDbContext db, Guid eventId)
    {
        return db.EventosProcesados
            .AnyAsync(x => x.EventoId == eventId);
    }

    private static async Task GuardarEvento(InventarioDbContext db, Guid eventId, string nombreEvento, string referenciaNegocio, string payload)
    {
        db.EventosProcesados.Add(new Domain.Entities.EventoProcesado
        {
            EventoId = eventId,
            NombreEvento = nombreEvento,
            ReferenciaNegocio = referenciaNegocio,
            Payload = payload,
            FechaProcesamiento = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Consumer detenido.");

        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}