using BuildingBlocks.Observability.Options;
using BuildingBlocks.Observability.Validation;
using FluentAssertions;

namespace BuildingBlocks.Observability.Tests.Validation;

public class ObservabilityOptionsValidatorTests
{
    private readonly ObservabilityOptionsValidator _validator = new();

    [Fact]
    public void Should_Fail_When_ApplicationName_Is_Empty()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ApplicationName = string.Empty,
            MinimumLevel = "Information",
            EnableConsole = true,
            EnableSeq = false
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("ApplicationName is required.");
    }

    [Fact]
    public void Should_Fail_When_MinimumLevel_Is_Empty()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ApplicationName = "TransaccionService",
            MinimumLevel = string.Empty,
            EnableConsole = true,
            EnableSeq = false
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("MinimumLevel is required.");
    }

    [Fact]
    public void Should_Fail_When_Seq_Is_Enabled_And_SeqUrl_Is_Empty()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ApplicationName = "TransaccionService",
            MinimumLevel = "Information",
            EnableConsole = true,
            EnableSeq = true,
            SeqUrl = string.Empty
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("SeqUrl is required when Seq is enabled.");
    }

    [Fact]
    public void Should_Succeed_When_Options_Are_Valid()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ApplicationName = "TransaccionService",
            MinimumLevel = "Information",
            EnableConsole = true,
            EnableSeq = true,
            SeqUrl = "http://localhost:5341"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }
}