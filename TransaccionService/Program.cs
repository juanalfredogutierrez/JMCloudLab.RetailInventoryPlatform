
using BuildingBlocks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);
StartupConsoleExtensions.PrintStartupInfo(builder.Environment.ApplicationName,
                                          builder.Environment.EnvironmentName,
                                          builder.Configuration);

builder.Host.AddBuildingBlockObservability();

builder.Services.AddBuildingBlocksOpenTelemetry(
    builder.Configuration,
    builder.Environment,
    builder.Logging);

builder.Services
    .AddApi()
    .AddApplication()
    .AddBuildingBlocks()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddObservabilityServices(builder.Configuration);
var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseApi();

app.Run();