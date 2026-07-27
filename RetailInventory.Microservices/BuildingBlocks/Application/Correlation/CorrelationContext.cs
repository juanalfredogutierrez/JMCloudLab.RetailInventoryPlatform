namespace BuildingBlocks.Correlation;

public static class CorrelationContext
{
    private static readonly AsyncLocal<string> _traceId = new();

    public static string TraceId
    {
        get => _traceId.Value ??= Guid.NewGuid().ToString();
        set => _traceId.Value = value;
    }
}