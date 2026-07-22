using BuildingBlocks.Observability.Behaviors;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Observability;

public static class DependencyInjection
{
    public static IServiceCollection AddObservabilityServices(
        this IServiceCollection services,IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICorrelationContext, CorrelationContext>();
        services.AddTransient(typeof(IPipelineBehavior<,>),
                         typeof(LoggingBehavior<,>));

        services.AddObservability(configuration);

        return services;
    }

    public static ConfigureHostBuilder AddBuildingBlockObservability(this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.AddObservability();
        return hostBuilder;
    }
}