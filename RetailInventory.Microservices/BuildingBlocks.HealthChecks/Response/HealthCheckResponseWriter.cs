using System.Text.Json;
using BuildingBlocks.HealthChecks.Response.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.HealthChecks.Response;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static async Task WriteResponse(
        HttpContext context,
        HealthReport report)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(report);

        context.Response.ContentType = "application/json";

        var response = new HealthResponse
        {
            Status = report.Status.ToString(),

            TotalDuration = report.TotalDuration,

            Checks = report.Entries
                .Select(entry => new HealthCheckEntryResponse
                {
                    Name = entry.Key,

                    Status = entry.Value.Status.ToString(),

                    Duration = entry.Value.Duration,

                    Description = entry.Value.Description
                })
                .ToList()
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions));
    }
}