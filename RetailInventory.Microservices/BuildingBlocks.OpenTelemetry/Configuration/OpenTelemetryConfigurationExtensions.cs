using BuildingBlocks.OpenTelemetry.Options;
using BuildingBlocks.OpenTelemetry.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.OpenTelemetry.Configuration;

internal static class OpenTelemetryConfigurationExtensions
{
    internal static OpenTelemetryOptions GetOpenTelemetryOptions(
        this IConfiguration configuration,
        IHostEnvironment environment)
    {
        var options = new OpenTelemetryOptions();

        configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Bind(options);

        options.ServiceName = environment.ApplicationName;
        options.Environment = environment.EnvironmentName;

        OpenTelemetryOptionsValidator.Validate(options);

        return options;
    }
}