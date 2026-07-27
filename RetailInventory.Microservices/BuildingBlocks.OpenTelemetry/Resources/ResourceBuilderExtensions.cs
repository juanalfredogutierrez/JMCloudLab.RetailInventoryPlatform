using BuildingBlocks.OpenTelemetry.Constants;
using BuildingBlocks.OpenTelemetry.Options;
using OpenTelemetry.Resources;

namespace BuildingBlocks.OpenTelemetry.Resources;

internal static class ResourceBuilderExtensions
{
    internal static ResourceBuilder AddDefaultResource(
        this ResourceBuilder builder,
        OpenTelemetryOptions options)
    {
        return builder
            .AddService(
                serviceName: options.ServiceName,
                serviceVersion: options.ServiceVersion)
            .AddAttributes(new Dictionary<string, object>
            {
                [ResourceAttributes.ServiceNamespace] = options.ServiceNamespace,
                [ResourceAttributes.DeploymentEnvironment] = options.Environment,
                [ResourceAttributes.ServiceInstanceId] = Environment.MachineName,
                [ResourceAttributes.HostName] = Environment.MachineName
            });
    }
}