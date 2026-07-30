using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace BuildingBlocks.HealthChecks.HealthChecks;

public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public RabbitMqHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionString =
                _configuration.GetConnectionString("RabbitMq");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return HealthCheckResult.Unhealthy(
                    "RabbitMQ connection string was not found.");
            }

            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            await using var connection =
                await factory.CreateConnectionAsync(cancellationToken);

            await using var channel =
                await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy(
                "RabbitMQ connection established successfully.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
               ex.ToString(),
    ex); 
        }
    }
}