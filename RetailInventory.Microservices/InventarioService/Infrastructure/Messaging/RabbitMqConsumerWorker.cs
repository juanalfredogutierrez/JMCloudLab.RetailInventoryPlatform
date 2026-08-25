using BuildingBlocks.Messaging;
using InventarioService.Application.Events.CompraRegistrada;
using InventarioService.Application.Events.VentaRegistrada;

namespace InventarioService.Infrastructure.Messaging;

public sealed class RabbitMqConsumerWorker : BackgroundService
{
    private const string CompraRegistradaRoutingKey = "compra.registrada";
    private const string VentaRegistradaRoutingKey = "venta.registrada";

    private readonly IMessageConsumer _messageConsumer;
    private readonly CompraRegistradaHandler _compraRegistradaHandler;
    private readonly VentaRegistradaHandler _ventaRegistradaHandler;
    private readonly ILogger<RabbitMqConsumerWorker> _logger;

    public RabbitMqConsumerWorker(
        IMessageConsumer messageConsumer,
        CompraRegistradaHandler compraRegistradaHandler,
        VentaRegistradaHandler ventaRegistradaHandler,
        ILogger<RabbitMqConsumerWorker> logger)
    {
        _messageConsumer = messageConsumer;
        _compraRegistradaHandler = compraRegistradaHandler;
        _ventaRegistradaHandler = ventaRegistradaHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "RabbitMQ Consumer Worker iniciado.");

        await _messageConsumer.StartAsync(
            ProcesarMensajeAsync,
            stoppingToken);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "RabbitMQ Consumer Worker detenido.");
        }
    }

    private async Task ProcesarMensajeAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        switch (context.RoutingKey)
        {
            case CompraRegistradaRoutingKey:

                await _compraRegistradaHandler.HandleAsync(
                    context,
                    cancellationToken);

                break;

            case VentaRegistradaRoutingKey:

                await _ventaRegistradaHandler.HandleAsync(
                    context,
                    cancellationToken);

                break;

            default:

                throw new InvalidOperationException(
                    $"RoutingKey no soportado: {context.RoutingKey}");
        }
    }
}