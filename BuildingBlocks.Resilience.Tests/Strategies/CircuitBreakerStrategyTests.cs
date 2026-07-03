using BuildingBlocks.Resilience.Options;
using FluentAssertions;
using Microsoft.Extensions.Http.Resilience;

namespace BuildingBlocks.Resilience.Tests.Strategies;

public class CircuitBreakerStrategyTests
{
    [Fact]
    public void Configure_Should_Set_All_Options()
    {
        // Arrange
        var strategyOptions = new HttpCircuitBreakerStrategyOptions();

        var options = new CircuitBreakerOptions
        {
            FailureRatio = 0.75,
            MinimumThroughput = 20,
            SamplingDurationInSeconds = 60,
            BreakDurationInSeconds = 30
        };

        // Act
        CircuitBreakerStrategy.Configure(strategyOptions, options);

        // Assert
        strategyOptions.FailureRatio.Should().Be(0.75);
        strategyOptions.MinimumThroughput.Should().Be(20);
        strategyOptions.SamplingDuration.Should().Be(TimeSpan.FromSeconds(60));
        strategyOptions.BreakDuration.Should().Be(TimeSpan.FromSeconds(30));
    }
}