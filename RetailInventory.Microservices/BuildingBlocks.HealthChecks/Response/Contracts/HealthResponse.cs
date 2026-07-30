namespace BuildingBlocks.HealthChecks.Response.Models;

public sealed class HealthResponse
{
    public string Status { get; init; } = string.Empty;

    public TimeSpan TotalDuration { get; init; }

    public IReadOnlyCollection<HealthCheckEntryResponse> Checks { get; init; }
        = [];
}