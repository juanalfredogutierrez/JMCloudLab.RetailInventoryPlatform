using BuildingBlocks.Resilience.Options;
using BuildingBlocks.Resilience.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Resilience.Options;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers resilience services and configuration.
    /// </summary>
    public static IServiceCollection AddResilience(this IServiceCollection services,IConfiguration configuration)
    {
        services
            .AddOptions<ResilienceOptions>()
            .Bind(configuration.GetSection(ResilienceOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<ResilienceOptions>,
            ResilienceOptionsValidator>();

        return services;
    }
}