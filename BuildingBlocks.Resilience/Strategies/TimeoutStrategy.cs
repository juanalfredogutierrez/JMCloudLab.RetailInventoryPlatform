
using BuildingBlocks.Resilience.Options;
using Microsoft.Extensions.Http.Resilience;

namespace BuildingBlocks.Resilience;

internal static class TimeoutStrategy
{
    public static void Configure( HttpTimeoutStrategyOptions options,TimeoutOptions timeoutOptions)
    {
        options.Timeout = TimeSpan.FromSeconds(timeoutOptions.TimeoutInSeconds);
    }
}