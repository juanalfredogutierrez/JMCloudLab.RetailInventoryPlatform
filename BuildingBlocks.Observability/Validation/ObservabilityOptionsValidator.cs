using BuildingBlocks.Observability.Options;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Observability.Validation;

internal sealed class ObservabilityOptionsValidator : IValidateOptions<ObservabilityOptions>
{
    public ValidateOptionsResult Validate( string? name,ObservabilityOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ApplicationName))
        {
            failures.Add("ApplicationName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.MinimumLevel))
        {
            failures.Add("MinimumLevel is required.");
        }

        if (options.EnableSeq &&
            string.IsNullOrWhiteSpace(options.SeqUrl))
        {
            failures.Add("SeqUrl is required when Seq is enabled.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}