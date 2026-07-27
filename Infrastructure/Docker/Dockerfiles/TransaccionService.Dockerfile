FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Solution
COPY RetailInventory.Microservices/Directory.Packages.props .
COPY RetailInventory.Microservices/JMCloudLab.RetailInventoryPlatform.sln .

# Shared Projects
COPY RetailInventory.Microservices/BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/
COPY RetailInventory.Microservices/BuildingBlocks.HealthChecks/BuildingBlocks.HealthChecks.csproj BuildingBlocks.HealthChecks/
COPY RetailInventory.Microservices/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj BuildingBlocks.Messaging/
COPY RetailInventory.Microservices/BuildingBlocks.Observability/BuildingBlocks.Observability.csproj BuildingBlocks.Observability/
COPY RetailInventory.Microservices/BuildingBlocks.OpenTelemetry/BuildingBlocks.OpenTelemetry.csproj BuildingBlocks.OpenTelemetry/
COPY RetailInventory.Microservices/BuildingBlocks.Resilience/BuildingBlocks.Resilience.csproj BuildingBlocks.Resilience/

# Service
COPY RetailInventory.Microservices/TransaccionService/TransaccionService.csproj TransaccionService/

RUN dotnet restore TransaccionService/TransaccionService.csproj

COPY RetailInventory.Microservices/. .

WORKDIR /src/TransaccionService

RUN dotnet publish \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

USER app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet","TransaccionService.dll"]