

namespace Gateway.OcelotGateway.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<WebApplication> UseGatewayAsync(
            this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AngularPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            await app.UseOcelot();

            return app;
        }
    }
}
