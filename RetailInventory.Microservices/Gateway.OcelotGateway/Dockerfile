FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Gateway.OcelotGateway/Gateway.OcelotGateway.csproj Gateway.OcelotGateway/
RUN dotnet restore Gateway.OcelotGateway/Gateway.OcelotGateway.csproj

COPY . .
WORKDIR /src/Gateway.OcelotGateway
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Gateway.OcelotGateway.dll"]