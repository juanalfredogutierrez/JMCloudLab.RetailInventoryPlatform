using BuildingBlocks.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocks(
     this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient(typeof(IPipelineBehavior<,>),
                                  typeof(ValidationBehavior<,>));

        return services;
    }
}