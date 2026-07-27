using BuildingBlocks.Observability.Options;
using BuildingBlocks.Observability.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Observability.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services,IConfiguration configuration)
    {
        services
            .AddOptions<ObservabilityOptions>()
            .Bind(configuration.GetSection(ObservabilityOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<ObservabilityOptions>,
            ObservabilityOptionsValidator>();

        return services;
    }
}