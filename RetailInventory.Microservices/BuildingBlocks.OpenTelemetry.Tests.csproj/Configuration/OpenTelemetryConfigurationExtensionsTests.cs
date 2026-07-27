using BuildingBlocks.OpenTelemetry.Configuration;
using BuildingBlocks.OpenTelemetry.Options;
using BuildingBlocks.OpenTelemetry.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace BuildingBlocks.OpenTelemetry.Tests.Configuration;

public class OpenTelemetryConfigurationExtensionsTests
{
    [Fact]
    public void GetOpenTelemetryOptions_Should_Bind_Configuration()
    {
        // Arrange
        var configuration = ConfigurationFactory.CreateValidConfiguration();
        var environment = new FakeHostEnvironment
        {
            EnvironmentName = "Development",
            ApplicationName = "ProductoService"
        };

        // Act
        var options = configuration.GetOpenTelemetryOptions(environment);

        // Assert
        options.Should().NotBeNull();

        options.ServiceNamespace.Should().Be("RetailInventory");
        options.ServiceVersion.Should().Be("1.0.0");
        options.Endpoint.Should().Be("http://localhost:4317");

        options.EnableTracing.Should().BeTrue();
        options.EnableMetrics.Should().BeTrue();
        options.EnableLogging.Should().BeTrue();

        options.EnableConsoleExporter.Should().BeFalse();
        options.EnableOtlpExporter.Should().BeTrue();
    }

    [Fact]
    public void GetOpenTelemetryOptions_Should_Set_Environment()
    {
        // Arrange
        var configuration = ConfigurationFactory.CreateValidConfiguration();

        var environment = new FakeHostEnvironment
        {
            EnvironmentName = Environments.Production,
            ApplicationName = "ProductoService"
        };

        // Act
        var options = configuration.GetOpenTelemetryOptions(environment);

        // Assert
        options.Environment.Should().Be(Environments.Production);
    }

    [Fact]
    public void GetOpenTelemetryOptions_Should_Set_ServiceName()
    {
        // Arrange
        var configuration = ConfigurationFactory.CreateValidConfiguration();

        var environment = new FakeHostEnvironment
        {
            ApplicationName = "ProductoService"
        };

        // Act
        var options = configuration.GetOpenTelemetryOptions(environment);

        // Assert
        options.ServiceName.Should().Be("ProductoService");
    }

    [Fact]
    public void GetOpenTelemetryOptions_Should_Throw_When_Configuration_Is_Invalid()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{OpenTelemetryOptions.SectionName}:ServiceNamespace"] = string.Empty,
                [$"{OpenTelemetryOptions.SectionName}:ServiceVersion"] = "1.0.0",
                [$"{OpenTelemetryOptions.SectionName}:Endpoint"] = "http://localhost:4317",
                [$"{OpenTelemetryOptions.SectionName}:EnableOtlpExporter"] = "true"
            })
            .Build();

        var environment = new FakeHostEnvironment
        {
            ApplicationName = "ProductoService",
            EnvironmentName = Environments.Development
        };

        // Act
        var action = () => configuration.GetOpenTelemetryOptions(environment);

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*ServiceNamespace is required*");
    }


}