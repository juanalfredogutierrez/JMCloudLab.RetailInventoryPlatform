using BuildingBlocks.Resilience;
using BuildingBlocks.Resilience.Extensions;


namespace TransaccionService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<TransaccionDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddBuildingBlockResilience(configuration);

            services.AddHttpClient("ProductoApi", client =>
            {
                client.BaseAddress = new Uri(configuration["Services:ProductoApi"]!);
            })
            .AddResilience(configuration);

            services.AddHttpClient("InventarioApi", client =>
            {
                client.BaseAddress = new Uri(configuration["Services:InventarioApi"]!);
            })
            .AddResilience(configuration);

            services.AddHostedService<OutboxPublisherWorker>();

            services.AddRabbitMq(configuration);

            return services;
        }
    }
}
