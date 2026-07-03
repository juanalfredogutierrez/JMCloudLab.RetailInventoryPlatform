using BuildingBlocks.Resilience.Options;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace BuildingBlocks.Resilience.Strategies;

internal static class RetryStrategy
{
    public static void Configure(HttpRetryStrategyOptions options, RetryOptions retryOptions)
    {
        options.MaxRetryAttempts = retryOptions.MaxRetryAttempts;
        options.Delay = TimeSpan.FromSeconds(retryOptions.DelayInSeconds);

        options.BackoffType = retryOptions.UseExponentialBackoff
                                        ? DelayBackoffType.Exponential
                                        : DelayBackoffType.Constant;

        options.UseJitter = true;
    }
}