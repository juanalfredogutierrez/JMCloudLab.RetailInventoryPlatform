using BuildingBlocks.Resilience.Options;
using FluentAssertions;
using Microsoft.Extensions.Http.Resilience;

namespace BuildingBlocks.Resilience.Tests.Strategies;

public class TimeoutStrategyTests
{
    [Fact]
    public void Configure_Should_Set_Timeout()
    {
        // Arrange
        var strategyOptions = new HttpTimeoutStrategyOptions();

        var timeoutOptions = new TimeoutOptions
        {
            TimeoutInSeconds = 15
        };

        // Act
        TimeoutStrategy.Configure(strategyOptions, timeoutOptions);

        // Assert
        strategyOptions.Timeout.Should().Be(TimeSpan.FromSeconds(15));
    }
}