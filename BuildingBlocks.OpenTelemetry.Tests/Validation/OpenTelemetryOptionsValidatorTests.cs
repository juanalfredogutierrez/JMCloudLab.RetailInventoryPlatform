using BuildingBlocks.OpenTelemetry.Options;
using BuildingBlocks.OpenTelemetry.Validation;
using FluentAssertions;
using Xunit;

namespace BuildingBlocks.OpenTelemetry.Tests.Validation;

public class OpenTelemetryOptionsValidatorTests
{
    [Fact]
    public void Validate_Should_NotThrow_When_Options_Are_Valid()
    {
        // Arrange
        var options = CreateValidOptions();

        // Act
        var action = () => OpenTelemetryOptionsValidator.Validate(options);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void Validate_Should_Throw_When_ServiceName_Is_Empty()
    {
        // Arrange
        var options = CreateValidOptions();
        options.ServiceName = string.Empty;

        // Act
        var action = () => OpenTelemetryOptionsValidator.Validate(options);

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*ServiceName is required*");
    }

    [Fact]
    public void Validate_Should_Throw_When_ServiceVersion_Is_Empty()
    {
        // Arrange
        var options = CreateValidOptions();
        options.ServiceVersion = string.Empty;

        // Act
        var action = () => OpenTelemetryOptionsValidator.Validate(options);

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*ServiceVersion is required*");
    }

    [Fact]
    public void Validate_Should_Throw_When_ServiceNamespace_Is_Empty()
    {
        // Arrange
        var options = CreateValidOptions();
        options.ServiceNamespace = string.Empty;

        // Act
        var action = () => OpenTelemetryOptionsValidator.Validate(options);

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*ServiceNamespace is required*");
    }

    [Fact]
    public void Validate_Should_Throw_When_Endpoint_Is_Empty_And_OtlpExporter_Is_Enabled()
    {
        // Arrange
        var options = CreateValidOptions();
        options.Endpoint = string.Empty;
        options.EnableOtlpExporter = true;

        // Act
        var action = () => OpenTelemetryOptionsValidator.Validate(options);

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*Endpoint is required*");
    }

    [Fact]
    public void Validate_Should_NotThrow_When_Endpoint_Is_Empty_And_OtlpExporter_Is_Disabled()
    {
        // Arrange
        var options = CreateValidOptions();
        options.Endpoint = string.Empty;
        options.EnableOtlpExporter = false;

        // Act
        var action = () => OpenTelemetryOptionsValidator.Validate(options);

        // Assert
        action.Should().NotThrow();
    }

    private static OpenTelemetryOptions CreateValidOptions()
    {
        var options = new OpenTelemetryOptions
        {
            ServiceNamespace = "RetailInventory",
            ServiceVersion = "1.0.0",
            Endpoint = "http://localhost:4317",
            EnableOtlpExporter = true
        };

        options.ServiceName = "ProductoService";

        return options;
    }
}