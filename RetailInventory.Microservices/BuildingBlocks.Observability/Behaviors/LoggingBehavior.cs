using BuildingBlocks.Observability.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Observability.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICorrelationContext _correlationContext;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICorrelationContext correlationContext)
    {
        _logger = logger;
        _correlationContext = correlationContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var traceId = _correlationContext.CorrelationId;

        var requestName = typeof(TRequest).Name;

        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "[START] {Request} | TraceId: {TraceId}",
            requestName,
            traceId);

        try
        {
            return await next();
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "[END] {Request} | TraceId: {TraceId} | {Elapsed}ms",
                requestName,
                traceId,
                stopwatch.ElapsedMilliseconds);
        }
    }
}