using System.Diagnostics;

namespace BuildingBlocks.OpenTelemetry.Tracing;

public static class TracingExtensions
{
    public static Activity? StartActivity(
        string name,
        ActivityKind kind = ActivityKind.Internal)
    {
        return TracingSources.Default.StartActivity(name, kind);
    }
}