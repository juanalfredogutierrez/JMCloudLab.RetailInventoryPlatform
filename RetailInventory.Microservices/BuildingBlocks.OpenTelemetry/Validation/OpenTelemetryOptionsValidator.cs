using BuildingBlocks.OpenTelemetry.Options;

namespace BuildingBlocks.OpenTelemetry.Validation;

internal static class OpenTelemetryOptionsValidator
{
    internal static void Validate(OpenTelemetryOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ServiceName))
            throw new InvalidOperationException("OpenTelemetry: ServiceName is required.");

        if (string.IsNullOrWhiteSpace(options.ServiceVersion))
            throw new InvalidOperationException("OpenTelemetry: ServiceVersion is required.");

        if (string.IsNullOrWhiteSpace(options.ServiceNamespace))
            throw new InvalidOperationException("OpenTelemetry: ServiceNamespace is required.");

        if (options.EnableOtlpExporter &&
            string.IsNullOrWhiteSpace(options.Endpoint))
        {
            throw new InvalidOperationException(
                "OpenTelemetry: Endpoint is required when OTLP exporter is enabled.");
        }
    }
}