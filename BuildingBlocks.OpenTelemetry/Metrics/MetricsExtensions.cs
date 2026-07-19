using System.Diagnostics.Metrics;

namespace BuildingBlocks.OpenTelemetry.Metrics;

public static class MetricsExtensions
{
    public static Counter<long> CreateCounter(
        string name,
        string? unit = null,
        string? description = null)
    {
        return MeterSources.Default.CreateCounter<long>(
            name,
            unit,
            description);
    }

    public static Histogram<double> CreateHistogram(
        string name,
        string? unit = null,
        string? description = null)
    {
        return MeterSources.Default.CreateHistogram<double>(
            name,
            unit,
            description);
    }

    public static UpDownCounter<long> CreateUpDownCounter(
        string name,
        string? unit = null,
        string? description = null)
    {
        return MeterSources.Default.CreateUpDownCounter<long>(
            name,
            unit,
            description);
    }
}