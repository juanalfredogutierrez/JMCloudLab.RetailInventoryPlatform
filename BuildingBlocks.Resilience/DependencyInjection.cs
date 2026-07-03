using BuildingBlocks.Resilience.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Resilience;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlockResilience(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddResilience(configuration);
        return services;
    }
}