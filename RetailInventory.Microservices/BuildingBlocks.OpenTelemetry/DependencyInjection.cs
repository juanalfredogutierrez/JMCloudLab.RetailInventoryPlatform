using BuildingBlocks.OpenTelemetry.Builders;
using BuildingBlocks.OpenTelemetry.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.OpenTelemetry;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocksOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        ILoggingBuilder logging)
    {
        var options = configuration.GetOpenTelemetryOptions(environment);

        logging.AddDefaultLogging(options);

        services.AddDefaultOpenTelemetry(options);

        return services;
    }
}