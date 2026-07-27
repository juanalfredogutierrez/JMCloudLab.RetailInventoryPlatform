namespace BuildingBlocks.Resilience.Options;

public sealed class ResilienceOptions
{
    public const string SectionName = "Resilience";

    public RetryOptions Retry { get; set; } = new();

    public TimeoutOptions Timeout { get; set; } = new();

    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();
}

public sealed class RetryOptions
{
    public int MaxRetryAttempts { get; set; } = 3;

    public int DelayInSeconds { get; set; } = 2;

    public bool UseExponentialBackoff { get; set; } = true;
}

public sealed class TimeoutOptions
{
    public int TimeoutInSeconds { get; set; } = 30;
}

public sealed class CircuitBreakerOptions
{
    public double FailureRatio { get; set; } = 0.5;

    public int MinimumThroughput { get; set; } = 10;

    public int SamplingDurationInSeconds { get; set; } = 30;

    public int BreakDurationInSeconds { get; set; } = 15;
}