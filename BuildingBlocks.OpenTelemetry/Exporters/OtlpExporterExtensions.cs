using BuildingBlocks.OpenTelemetry.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetry.Exporters;

internal static class OtlpExporterExtensions
{
    internal static TracerProviderBuilder AddDefaultExporters(
        this TracerProviderBuilder builder,
        OpenTelemetryOptions options)
    {
        if (options.EnableOtlpExporter)
        {
            builder.AddOtlpExporter(otlp =>
            {
                otlp.Endpoint = new Uri(options.Endpoint);
                otlp.Protocol = OtlpExportProtocol.Grpc;
            });
        }

        return builder;
    }
}