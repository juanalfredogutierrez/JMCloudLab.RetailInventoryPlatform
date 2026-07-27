using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "N/A";

        var requestName = typeof(TRequest).Name;

        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("[START] {Request} | TraceId: {TraceId}",
            requestName, traceId);

        try
        {
            return await next();
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation("[END] {Request} | TraceId: {TraceId} | {Elapsed}ms",
                requestName,
                traceId,
                stopwatch.ElapsedMilliseconds);
        }
    }
}