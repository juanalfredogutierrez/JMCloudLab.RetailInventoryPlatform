
using BuildingBlocks.OpenTelemetry;
using BuildingBlocks.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
StartupConsoleExtensions.PrintStartupInfo(builder.Environment.ApplicationName,
                                          builder.Environment.EnvironmentName,
                                          builder.Configuration);

builder.Host.AddBuildingBlockObservability();
builder.Services.AddPlatformHealthChecks(builder.Configuration);

builder.Services.AddBuildingBlocksOpenTelemetry(
    builder.Configuration,
    builder.Environment,
    builder.Logging);

builder.Services
        .AddApi()
        .AddApplication()
        .AddBuildingBlocks()
        .AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.MapPlatformHealthChecks();
await app.InitializeDatabaseAsync();

app.UseApi();

app.Run();