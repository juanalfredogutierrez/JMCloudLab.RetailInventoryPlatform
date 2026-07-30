namespace BuildingBlocks.HealthChecks.Constants;

public static class HealthCheckTags
{
    public const string Ready = "ready";

    public const string Live = "live";

    public const string Startup = "startup";

    public const string SqlServer = "sqlserver";

    public const string RabbitMq = "rabbitmq";

    public const string Redis = "redis";

    public const string External = "external";
}