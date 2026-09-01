using BuildingBlocks.Messaging;
using InventarioService.Application.Events.CompraRegistrada;
using InventarioService.Application.Events.VentaRegistrada;
namespace InventarioService.Infrastructure.Messaging
{

    public sealed class RabbitMqConsumerWorker : BackgroundService
    {
        private const string CompraRegistradaRoutingKey = "compra.registrada";
        private const string VentaRegistradaRoutingKey = "venta.registrada";

        private readonly IMessageConsumer _messageConsumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitMqConsumerWorker> _logger;

        public RabbitMqConsumerWorker(
            IMessageConsumer messageConsumer,
            IServiceScopeFactory scopeFactory,
            ILogger<RabbitMqConsumerWorker> logger)
        {
            _messageConsumer = messageConsumer;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Worker iniciado.");

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
            await using var scope =
                _scopeFactory.CreateAsyncScope();

            var compraHandler =
                scope.ServiceProvider
                    .GetRequiredService<CompraRegistradaHandler>();

            var ventaHandler =
                scope.ServiceProvider
                    .GetRequiredService<VentaRegistradaHandler>();

            switch (context.MessageType)
            {
                case CompraRegistradaRoutingKey:

                    await compraHandler.HandleAsync(
                        context,
                        cancellationToken);

                    break;

                case VentaRegistradaRoutingKey:

                    await ventaHandler.HandleAsync(
                        context,
                        cancellationToken);

                    break;

                default:

                    throw new InvalidOperationException(
                        $"MessageType no soportado: {context.MessageType}");
            }
        }
    }
}