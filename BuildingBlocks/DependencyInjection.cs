using BuildingBlocks.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocks(
     this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>),
                              typeof(ValidationBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>),
                              typeof(LoggingBehavior<,>));

        return services;
    }
}