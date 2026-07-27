using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetry.Instrumentation;

internal static class TracingInstrumentationExtensions
{
    internal static TracerProviderBuilder AddDefaultInstrumentation(
        this TracerProviderBuilder builder)
    {
        return builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    }
}