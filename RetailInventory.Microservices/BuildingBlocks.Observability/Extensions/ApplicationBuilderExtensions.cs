using BuildingBlocks.Observability.Middleware;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace BuildingBlocks.Observability.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBuildingBlockObservability(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationMiddleware>();
        app.UseSerilogRequestLogging();

        return app;
    }
}   