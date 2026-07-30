using BuildingBlocks.HealthChecks.Checks;
using BuildingBlocks.HealthChecks.HealthChecks;
using BuildingBlocks.HealthChecks.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.HealthChecks;

public static class HealthChecksBuilderExtensions
{
    public static IHealthChecksBuilder AddPlatformSelfHealthCheck(
        this IHealthChecksBuilder builder,
        PlatformHealthCheckOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        if (!options.EnableSelfCheck)
            return builder;

        builder.AddCheck<SelfHealthCheck>("self");

        return builder;
    }

    public static IHealthChecksBuilder AddPlatformSqlServerHealthCheck(
        this IHealthChecksBuilder builder,
        IConfiguration configuration,
        PlatformHealthCheckOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(options);

        if (!options.EnableSqlServer)
            return builder;

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        builder.AddSqlServer(
            connectionString,
            name: "sqlserver");

        return builder;
    }
    public static IHealthChecksBuilder AddPlatformRabbitMqHealthCheck(
        this IHealthChecksBuilder builder,
        IConfiguration configuration,
        PlatformHealthCheckOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(options);

        if (!options.EnableRabbitMq)
            return builder;

        builder.AddCheck<RabbitMqHealthCheck>("rabbitmq");

        return builder;
    }
}