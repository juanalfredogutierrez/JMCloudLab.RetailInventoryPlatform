using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetry.Exporters;

internal static class ConsoleExporterExtensions
{
    internal static TracerProviderBuilder AddConsoleExporterIfDevelopment(
        this TracerProviderBuilder builder,
        bool enabled)
    {
        if (enabled)
        {
            //builder.AddConsoleExporter();
        }

        return builder;
    }
}