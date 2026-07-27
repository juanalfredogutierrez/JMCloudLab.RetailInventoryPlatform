using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.RabbiMQ;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>( configuration.GetSection(
                RabbitMqOptions.SectionName));

        services.AddSingleton(sp =>
        {
            var options =
                sp.GetRequiredService<
                    IOptions<RabbitMqOptions>>().Value;

            var factory = new ConnectionFactory
            {
                HostName = options.Host,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };

            return factory.CreateConnectionAsync()
                          .GetAwaiter()
                          .GetResult();
        });

        services.AddScoped(sp =>
        {
            var connection =
                sp.GetRequiredService<IConnection>();

            return connection.CreateChannelAsync()
                             .GetAwaiter()
                             .GetResult();
        });

        services.AddScoped<IMessagePublisher,
            RabbitMqPublisher>();

        return services;
    }
}