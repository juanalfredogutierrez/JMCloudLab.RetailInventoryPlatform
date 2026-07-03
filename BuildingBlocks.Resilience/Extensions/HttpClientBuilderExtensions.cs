using BuildingBlocks.Resilience.Options;
using BuildingBlocks.Resilience.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Resilience.Extensions;

public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddResilience(this IHttpClientBuilder builder, IConfiguration configuration)
    {
        var resilienceOptions = configuration
            .GetSection(ResilienceOptions.SectionName)
            .Get<ResilienceOptions>() ?? new ResilienceOptions();

        builder.AddStandardResilienceHandler(options =>
        {
            RetryStrategy.Configure(options.Retry,resilienceOptions.Retry);

            TimeoutStrategy.Configure(options.TotalRequestTimeout, resilienceOptions.Timeout);

            CircuitBreakerStrategy.Configure(options.CircuitBreaker, resilienceOptions.CircuitBreaker);
        });

        return builder;
    }
}