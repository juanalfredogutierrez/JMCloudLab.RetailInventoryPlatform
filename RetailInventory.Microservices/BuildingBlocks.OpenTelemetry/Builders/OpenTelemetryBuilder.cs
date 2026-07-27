using BuildingBlocks.OpenTelemetry.Options;
using BuildingBlocks.OpenTelemetry.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.OpenTelemetry.Builders;

internal static class OpenTelemetryBuilder
{
    internal static IServiceCollection AddDefaultOpenTelemetry(
        this IServiceCollection services,
        OpenTelemetryOptions options)
    {
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

        return services;
    }
}