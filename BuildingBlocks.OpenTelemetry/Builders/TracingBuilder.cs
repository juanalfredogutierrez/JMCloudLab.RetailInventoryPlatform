using BuildingBlocks.OpenTelemetry.Tracing;
using BuildingBlocks.OpenTelemetry.Exporters;
using BuildingBlocks.OpenTelemetry.Instrumentation;
using BuildingBlocks.OpenTelemetry.Options;
using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetry.Builders;

internal static class TracingBuilder
{
    internal static TracerProviderBuilder AddDefaultTracing(
        this TracerProviderBuilder builder,
        OpenTelemetryOptions options)
    {
        return builder
            .AddSource(TracingSources.Name)
            .AddDefaultInstrumentation()
            .AddDefaultExporters(options);
    }
}