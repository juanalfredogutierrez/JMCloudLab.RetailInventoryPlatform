namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
    
        services.AddScoped<PasswordHasher<Usuario>>();
        services.AddScoped<JwtTokenGenerator>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        return services;
    }
}