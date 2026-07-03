using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BuildingBlocks.Resilience.Validation;
using BuildingBlocks.Resilience.Options;

namespace BuildingBlocks.Resilience;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers resilience services and configuration.
    /// </summary>
    /// 
    public static IServiceCollection AddBuildingBlockResilience(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<ResilienceOptions>()
            .Bind(configuration.GetSection(ResilienceOptions.SectionName))
            .ValidateOnStart();
        services.AddResilience(configuration);
        services.AddSingleton<IValidateOptions<ResilienceOptions>,ResilienceOptionsValidator>();

        return services;
    }
}

