using BuildingBlocks.Resilience.Options;
using FluentAssertions;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace BuildingBlocks.Resilience.Tests.Strategies;

public class RetryStrategyTests
{
    [Fact]
    public void Configure_Should_Set_All_Retry_Options()
    {
        // Arrange
        var strategyOptions = new HttpRetryStrategyOptions();

        var retryOptions = new RetryOptions
        {
            MaxRetryAttempts = 5,
            DelayInSeconds = 3,
            UseExponentialBackoff = true
        };

        // Act
        RetryStrategy.Configure(strategyOptions, retryOptions);

        // Assert
        strategyOptions.MaxRetryAttempts.Should().Be(5);
        strategyOptions.Delay.Should().Be(TimeSpan.FromSeconds(3));
        strategyOptions.BackoffType.Should().Be(DelayBackoffType.Exponential);
        strategyOptions.UseJitter.Should().BeTrue();
    }

    [Fact]
    public void Configure_Should_Use_Constant_Backoff_When_Disabled()
    {
        // Arrange
        var strategyOptions = new HttpRetryStrategyOptions();

        var retryOptions = new RetryOptions
        {
            MaxRetryAttempts = 2,
            DelayInSeconds = 1,
            UseExponentialBackoff = false
        };

        // Act
        RetryStrategy.Configure(strategyOptions, retryOptions);

        // Assert
        strategyOptions.BackoffType.Should().Be(DelayBackoffType.Constant);
    }
}