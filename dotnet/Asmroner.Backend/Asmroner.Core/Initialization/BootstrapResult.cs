using Asmroner.Core.Configuration;

namespace Asmroner.Core.Initialization;

public sealed class BootstrapResult
{
    private BootstrapResult()
    {
    }

    public bool IsSuccess { get; private init; }

    public bool RequiresSetup { get; private init; }

    public string? ErrorMessage { get; private init; }

    public AppConfig? Config { get; private init; }

    public IReadOnlyList<string> ValidationErrors { get; private init; } = Array.Empty<string>();

    public static BootstrapResult Success(AppConfig config)
    {
        return new BootstrapResult
        {
            IsSuccess = true,
            Config = config,
        };
    }

    public static BootstrapResult SetupRequired(AppConfig config, IReadOnlyList<string> validationErrors)
    {
        return new BootstrapResult
        {
            IsSuccess = false,
            RequiresSetup = true,
            Config = config,
            ValidationErrors = validationErrors,
            ErrorMessage = validationErrors.Count > 0 ? string.Join("; ", validationErrors) : null,
        };
    }

    public static BootstrapResult Failed(string errorMessage)
    {
        return new BootstrapResult
        {
            IsSuccess = false,
            RequiresSetup = false,
            ErrorMessage = errorMessage,
        };
    }
}