using BuildingBlocks.HealthChecks.Response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.HealthChecks;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapPlatformHealthChecks(
        this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapHealthChecks(
            "/health",
            new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteResponse
            });

        return endpoints;
    }
}