using BuildingBlocks.Resilience.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Resilience.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResilience(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ResilienceOptions>(configuration.GetSection(ResilienceOptions.SectionName));

        return services;
    }
}