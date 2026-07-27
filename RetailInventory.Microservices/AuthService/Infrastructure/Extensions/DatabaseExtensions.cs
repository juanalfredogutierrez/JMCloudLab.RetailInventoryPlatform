namespace AuthService.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            await db.Database.MigrateAsync();
            await AuthDbSeeder.SeedAsync(db);
        }
    }
}
