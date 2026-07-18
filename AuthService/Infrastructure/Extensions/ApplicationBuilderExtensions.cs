
using BuildingBlocks.Observability.Extensions;
namespace AuthService.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UseApi(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseBuildingBlockObservability();

            app.MapControllers();

            return app;
        }
    }
}
