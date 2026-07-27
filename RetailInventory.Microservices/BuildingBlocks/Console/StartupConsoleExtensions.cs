using Microsoft.Extensions.Configuration;

public static class StartupConsoleExtensions
{
    public static void PrintStartupInfo(
        string appName,
        string environment,
        IConfiguration configuration)
    {
        var color = GetServiceColor(configuration, appName);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("[");

        Console.ForegroundColor = color;
        Console.Write(appName);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("] ");

        Console.ForegroundColor = environment switch
        {
            "Development" => ConsoleColor.Green,
            "Docker" => ConsoleColor.Yellow,
            "Production" => ConsoleColor.Red,
            _ => ConsoleColor.White
        };

        Console.WriteLine(environment);

        Console.ResetColor();
    }

    private static ConsoleColor GetServiceColor(
        IConfiguration configuration,
        string appName)
    {
        var configuredColor = configuration["Console:ServiceColor"];

        if (!string.IsNullOrWhiteSpace(configuredColor) &&
            Enum.TryParse(configuredColor, true, out ConsoleColor color))
        {
            return color;
        }

        return ConsoleColor.Cyan;
    }
}