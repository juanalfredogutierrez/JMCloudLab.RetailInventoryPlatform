
using BuildingBlocks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    $"ocelot.{builder.Environment.EnvironmentName}.json",
    optional: false,
    reloadOnChange: true);

StartupConsoleExtensions.PrintStartupInfo(
    builder.Environment.ApplicationName,
    builder.Environment.EnvironmentName,
    builder.Configuration);

builder.Host.AddBuildingBlockObservability();
builder.Services.AddBuildingBlocksOpenTelemetry(
    builder.Configuration,
    builder.Environment,
    builder.Logging);
builder.Services.AddGateway(builder.Configuration);

var app = builder.Build();

await app.UseGatewayAsync();

app.Run();