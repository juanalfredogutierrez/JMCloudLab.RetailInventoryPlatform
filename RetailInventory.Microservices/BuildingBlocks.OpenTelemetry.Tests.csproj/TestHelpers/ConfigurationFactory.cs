using BuildingBlocks.OpenTelemetry.Options;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.OpenTelemetry.Tests.TestHelpers;

internal static class ConfigurationFactory
{
    public static IConfiguration CreateValidConfiguration()
    {
        var settings = new Dictionary<string, string>
        {
            [$"{OpenTelemetryOptions.SectionName}:ServiceNamespace"] = "RetailInventory",
            [$"{OpenTelemetryOptions.SectionName}:ServiceVersion"] = "1.0.0",
            [$"{OpenTelemetryOptions.SectionName}:Endpoint"] = "http://localhost:4317",
            [$"{OpenTelemetryOptions.SectionName}:EnableTracing"] = "true",
            [$"{OpenTelemetryOptions.SectionName}:EnableMetrics"] = "true",
            [$"{OpenTelemetryOptions.SectionName}:EnableLogging"] = "true",
            [$"{OpenTelemetryOptions.SectionName}:EnableConsoleExporter"] = "false",
            [$"{OpenTelemetryOptions.SectionName}:EnableOtlpExporter"] = "true"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}