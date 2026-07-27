using BuildingBlocks.OpenTelemetry.Options;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

namespace BuildingBlocks.OpenTelemetry.Builders;

internal static class LoggingBuilder
{
    internal static ILoggingBuilder AddDefaultLogging(
        this ILoggingBuilder builder,
        OpenTelemetryOptions options)
    {
        builder.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;

            if (options.EnableOtlpExporter)
            {
                logging.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(options.Endpoint);
                    otlp.Protocol = OtlpExportProtocol.Grpc;
                });
            }
        });

        return builder;
    }
}