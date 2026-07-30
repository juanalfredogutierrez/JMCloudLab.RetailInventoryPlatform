using BuildingBlocks.HealthChecks.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.HealthChecks;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatformHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new PlatformHealthCheckOptions();

        configuration
            .GetSection("HealthChecks")
            .Bind(options);

        services
            .AddHealthChecks()
            .AddPlatformSelfHealthCheck(options)
            .AddPlatformSqlServerHealthCheck(configuration, options)
            .AddPlatformRabbitMqHealthCheck(configuration, options);

        return services;
    }
}