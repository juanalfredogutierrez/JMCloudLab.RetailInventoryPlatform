namespace TransaccionService.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<TransaccionDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
