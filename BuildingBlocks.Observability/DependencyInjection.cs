using BuildingBlocks.Observability.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Observability;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlockObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddObservability(configuration);

        return services;
    }

    public static ConfigureHostBuilder AddBuildingBlockObservability(
        this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.AddObservability();

        return hostBuilder;
    }
}