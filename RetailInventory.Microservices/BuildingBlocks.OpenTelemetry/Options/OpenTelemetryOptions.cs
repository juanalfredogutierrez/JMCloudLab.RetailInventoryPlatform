namespace BuildingBlocks.OpenTelemetry.Options;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public string ServiceNamespace { get; set; } = string.Empty;

    public string ServiceName { get; internal set; } = string.Empty;

    public string ServiceVersion { get; set; } = "1.0.0";

    public string Environment { get; internal set; } = string.Empty;

    public string Endpoint { get; set; } = "http://localhost:4317";

    public bool EnableTracing { get; set; } = true;

    public bool EnableMetrics { get; set; } = true;

    public bool EnableLogging { get; set; } = true;

    public bool EnableConsoleExporter { get; set; } = false;

    public bool EnableOtlpExporter { get; set; } = true;
}