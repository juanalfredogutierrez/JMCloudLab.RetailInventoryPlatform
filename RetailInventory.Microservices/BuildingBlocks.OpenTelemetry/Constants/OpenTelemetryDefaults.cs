namespace BuildingBlocks.OpenTelemetry.Constants;

internal static class OpenTelemetryDefaults
{
    internal const string ServiceName = "UnknownService";
    internal const string ServiceVersion = "1.0.0";
    internal const string ActivitySourceName = "BuildingBlocks.OpenTelemetry";

    internal const string TracesEndpoint = "http://localhost:4317";
    internal const string MetricsEndpoint = "http://localhost:4317";
    internal const string LogsEndpoint = "http://localhost:4317";
}