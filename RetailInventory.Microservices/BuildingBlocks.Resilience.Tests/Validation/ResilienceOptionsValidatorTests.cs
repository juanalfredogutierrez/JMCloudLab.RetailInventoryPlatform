using BuildingBlocks.Resilience.Options;
using BuildingBlocks.Resilience.Validation;
using FluentAssertions;

namespace BuildingBlocks.Resilience.Tests.Validation;

public class ResilienceOptionsValidatorTests
{
    private readonly ResilienceOptionsValidator _validator = new();

    [Fact]
    public void Validate_Should_ReturnSuccess_When_ConfigurationIsValid()
    {
        // Arrange

        var options = new ResilienceOptions
        {
            Retry = new RetryOptions
            {
                MaxRetryAttempts = 3,
                DelayInSeconds = 2
            },
            Timeout = new TimeoutOptions
            {
                TimeoutInSeconds = 10
            },
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                SamplingDurationInSeconds = 20,
                BreakDurationInSeconds = 30
            }
        };

        // Act

        var result = _validator.Validate(null, options);

        // Assert

        result.Failed.Should().BeFalse();
    }

    [Fact]
    public void Validate_Should_Fail_When_SamplingDuration_IsLessThanDoubleTimeout()
    {
        // Arrange

        var options = new ResilienceOptions
        {
            Timeout = new TimeoutOptions
            {
                TimeoutInSeconds = 10
            },
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                SamplingDurationInSeconds = 10,
                BreakDurationInSeconds = 30
            }
        };

        // Act

        var result = _validator.Validate(null, options);

        // Assert

        result.Failed.Should().BeTrue();

        result.Failures.Should()
            .Contain(x =>
                x.Contains("SamplingDurationInSeconds"));
    }

    [Fact]
    public void Validate_Should_Fail_When_MaxRetryAttempts_IsNegative()
    {
        var options = CreateValidOptions();

        options.Retry.MaxRetryAttempts = -1;

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_When_RetryDelay_IsZero()
    {
        var options = CreateValidOptions();

        options.Retry.DelayInSeconds = 0;

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_When_Timeout_IsZero()
    {
        var options = CreateValidOptions();

        options.Timeout.TimeoutInSeconds = 0;

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(2)]
    public void Validate_ShouldFail_WhenFailureRatioIsInvalid(double ratio)
    {
        var options = CreateValidOptions();

        options.CircuitBreaker.FailureRatio = ratio;

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_When_MinimumThroughput_IsZero()
    {
        var options = CreateValidOptions();

        options.CircuitBreaker.MinimumThroughput = 0;

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
    }
    private static ResilienceOptions CreateValidOptions()
    {
        return new ResilienceOptions
        {
            Retry = new RetryOptions
            {
                MaxRetryAttempts = 3,
                DelayInSeconds = 2
            },
            Timeout = new TimeoutOptions
            {
                TimeoutInSeconds = 10
            },
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                SamplingDurationInSeconds = 20,
                BreakDurationInSeconds = 30
            }
        };
    }

}