using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.HealthChecks;
public static class DependencyInjection
{
   
        public static IServiceCollection AddPlatformHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            var builder = services.AddHealthChecks();

            return services;
        }
    
}
