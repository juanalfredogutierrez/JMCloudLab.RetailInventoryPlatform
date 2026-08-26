using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messaging.RabbitMQ;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(
            configuration.GetSection(
                RabbitMqOptions.SectionName));

        services.AddSingleton(sp =>
            sp.GetRequiredService<
                IOptions<RabbitMqOptions>>().Value);

        services.AddSingleton<
            IMessagePublisher,
            RabbitMqPublisher>();

        services.AddSingleton<IMessageConsumer>(sp =>
        {
            var options =
                sp.GetRequiredService<RabbitMqOptions>();

            var logger =
                sp.GetRequiredService<
                    ILogger<RabbitMqConsumer>>();

            return new RabbitMqConsumer(
                options,
                logger);
        });

        return services;
    }
}