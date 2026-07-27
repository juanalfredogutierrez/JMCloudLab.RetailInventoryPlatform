using System.Diagnostics;

namespace BuildingBlocks.OpenTelemetry.Tracing;

internal static class TracingSources
{
    public const string Name = "RetailInventory";
    public static readonly ActivitySource Default = new(Name);
}