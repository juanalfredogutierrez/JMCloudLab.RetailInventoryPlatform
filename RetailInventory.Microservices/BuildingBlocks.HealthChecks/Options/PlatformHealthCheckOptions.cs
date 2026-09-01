namespace BuildingBlocks.HealthChecks.Options;

public sealed class PlatformHealthCheckOptions
{
    public bool EnableSelfCheck { get; set; } = true;

    public bool EnableSqlServer { get; set; }

    public bool EnableRabbitMq { get; set; }
}