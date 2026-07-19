
var builder = WebApplication.CreateBuilder(args);

builder.Host.AddBuildingBlockObservability();

builder.Services
        .AddApi()
        .AddApplication()
        .AddBuildingBlocks()
        .AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseApi();

app.Run();