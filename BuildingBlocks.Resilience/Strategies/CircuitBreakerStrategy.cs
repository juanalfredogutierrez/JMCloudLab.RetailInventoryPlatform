using BuildingBlocks.Resilience.Options;
using Microsoft.Extensions.Http.Resilience;

namespace BuildingBlocks.Resilience.Strategies;

internal static class CircuitBreakerStrategy
{
    public static void Configure( HttpCircuitBreakerStrategyOptions options,CircuitBreakerOptions circuitBreakerOptions)
    {
        options.FailureRatio = circuitBreakerOptions.FailureRatio;

        options.MinimumThroughput = circuitBreakerOptions.MinimumThroughput;

        options.SamplingDuration = TimeSpan.FromSeconds(circuitBreakerOptions.SamplingDurationInSeconds);

        options.BreakDuration = TimeSpan.FromSeconds(circuitBreakerOptions.BreakDurationInSeconds);
    }
}