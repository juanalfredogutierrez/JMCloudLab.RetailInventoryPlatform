namespace BuildingBlocks.HealthChecks.Response.Models;

public sealed class HealthCheckEntryResponse
{
    public string Name { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public TimeSpan Duration { get; init; }

    public string Description { get; init; }
}