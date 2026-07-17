using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Observability.Services;

public sealed class CorrelationContext : ICorrelationContext
{
    private static readonly AsyncLocal<string> _correlationId = new();

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationContext(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CorrelationId
    {
        get
        {
            if (_httpContextAccessor.HttpContext is not null)
            {
                return _httpContextAccessor.HttpContext.TraceIdentifier;
            }

            return _correlationId.Value ??= Guid.NewGuid().ToString("N");
        }
    }

    public void SetCorrelationId(string correlationId)
    {
        if (_httpContextAccessor.HttpContext is not null)
        {
            _httpContextAccessor.HttpContext.TraceIdentifier = correlationId;
        }

        _correlationId.Value = correlationId;
    }
}