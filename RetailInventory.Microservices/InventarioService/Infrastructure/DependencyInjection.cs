
namespace InventarioService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                     IConfiguration configuration)
        {
            services.AddDbContext<InventarioDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddHostedService<RabbitMqConsumerWorker>();
            services.AddRabbitMq(configuration);

            return services;
        }
    }
}
