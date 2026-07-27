using Microsoft.Extensions.Logging;

public static class LoggerExtensions
{
    public static void LogBusiness(this ILogger logger,string message)
    {
        logger.LogInformation("[BUSINESS] {Message}", message);
    }

    public static void LogIntegration(this ILogger logger,string message)
    {
        logger.LogInformation("[INTEGRATION] {Message}", message);
    }
}