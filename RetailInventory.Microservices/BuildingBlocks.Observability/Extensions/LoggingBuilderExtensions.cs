using BuildingBlocks.Observability.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.Observability.Extensions;

public static class LoggingBuilderExtensions
{
    public static ConfigureHostBuilder AddObservability(
        this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            var options = context.Configuration
                .GetRequiredSection(ObservabilityOptions.SectionName)
                .Get<ObservabilityOptions>()!;

            var version = typeof(LoggingBuilderExtensions)
                .Assembly
                .GetName()
                .Version?
                .ToString() ?? "1.0.0";

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)

                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()

                .Enrich.WithProperty("Application", options.ApplicationName)
                .Enrich.WithProperty("ServiceName", options.ApplicationName)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("Version", version)

                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("LuckyPennySoftware", LogEventLevel.Error);

            if (options.EnableConsole)
            {
                loggerConfiguration.WriteTo.Console();
            }

            if (options.EnableSeq && !string.IsNullOrWhiteSpace(options.SeqUrl))
            {
                loggerConfiguration.WriteTo.Seq(options.SeqUrl);
            }
        });

        return hostBuilder;
    }
}