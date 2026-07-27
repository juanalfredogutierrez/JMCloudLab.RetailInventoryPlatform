

using BuildingBlocks.Resilience.Options;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Resilience.Validation;

internal sealed class ResilienceOptionsValidator : IValidateOptions<ResilienceOptions>
{
    public ValidateOptionsResult Validate( string? name, ResilienceOptions options)
    {
        var failures = new List<string>();

        if (options.Retry.MaxRetryAttempts < 0)
        {
            failures.Add("Retry:MaxRetryAttempts must be greater than or equal to 0.");
        }

        if (options.Retry.DelayInSeconds <= 0)
        {
            failures.Add("Retry:DelayInSeconds must be greater than 0.");
        }

        if (options.Timeout.TimeoutInSeconds <= 0)
        {
            failures.Add("Timeout:TimeoutInSeconds must be greater than 0.");
        }

        if (options.CircuitBreaker.FailureRatio <= 0 ||
            options.CircuitBreaker.FailureRatio > 1)
        {
            failures.Add("CircuitBreaker:FailureRatio must be between 0 and 1.");
        }

        if (options.CircuitBreaker.MinimumThroughput <= 0)
        {
            failures.Add("CircuitBreaker:MinimumThroughput must be greater than 0.");
        }

        if (options.CircuitBreaker.BreakDurationInSeconds <= 0)
        {
            failures.Add("CircuitBreaker:BreakDurationInSeconds must be greater than 0.");
        }

        if (options.CircuitBreaker.SamplingDurationInSeconds <
            options.Timeout.TimeoutInSeconds * 2)
        {
            failures.Add(
                $"CircuitBreaker:SamplingDurationInSeconds ({options.CircuitBreaker.SamplingDurationInSeconds}s) " +
                $"must be at least double Timeout:TimeoutInSeconds ({options.Timeout.TimeoutInSeconds}s).");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}