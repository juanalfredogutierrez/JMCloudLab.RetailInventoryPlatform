using BuildingBlocks.OpenTelemetry.Metrics;
using BuildingBlocks.OpenTelemetry.Options;
using OpenTelemetry.Metrics;

namespace BuildingBlocks.OpenTelemetry.Builders;

internal static class MetricsBuilder
{
    internal static MeterProviderBuilder AddDefaultMetrics(
        this MeterProviderBuilder builder,
        OpenTelemetryOptions options)
    {
        builder
            .AddMeter(MeterSources.Name)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();

        if (options.EnableOtlpExporter)
        {
            builder.AddOtlpExporter(otlp =>
            {
                otlp.Endpoint = new Uri(options.Endpoint);
            });
        }

        return builder;
    }
}