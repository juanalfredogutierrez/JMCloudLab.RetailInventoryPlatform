using BuildingBlocks.OpenTelemetry.Builders;
using BuildingBlocks.OpenTelemetry.Options;
using BuildingBlocks.OpenTelemetry.Resources;
using BuildingBlocks.OpenTelemetry.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.OpenTelemetry;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenTelemetryServices(
        this IServiceCollection services,
        ILoggingBuilder logging,
        Action<OpenTelemetryOptions> configure)
    {
        var options = new OpenTelemetryOptions();

        configure(options);

        OpenTelemetryOptionsValidator.Validate(options);

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddDefaultResource(options);
            })
            .WithTracing(tracing =>
            {
                tracing.AddDefaultTracing(options);
            })
            .WithMetrics(metrics =>
            {
                metrics.AddDefaultMetrics(options);
            });

        logging.AddDefaultLogging(options);

        return services;
    }
}