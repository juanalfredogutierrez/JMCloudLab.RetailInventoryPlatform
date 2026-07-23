using System.Diagnostics.Metrics;

namespace BuildingBlocks.OpenTelemetry.Metrics;

internal static class MeterSources
{
    public const string Name = "RetailInventory";

    public static readonly Meter Default = new(Name);
}   