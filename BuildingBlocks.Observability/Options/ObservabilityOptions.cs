namespace BuildingBlocks.Observability.Options;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public string ApplicationName { get; set; } = string.Empty;

    public string MinimumLevel { get; set; } = "Information";

    public bool EnableConsole { get; set; } = true;

    public bool EnableSeq { get; set; } = true;

    public string? SeqUrl { get; set; }
}