using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class AppPathService : IAppPathService
{
    public AppPathService(string? userHomeDirectory = null)
    {
        var usesDefaultProgramDirectory = string.IsNullOrWhiteSpace(userHomeDirectory);
        var resolvedHomeDirectory = usesDefaultProgramDirectory
            ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            : userHomeDirectory!;

        var programDirectory = usesDefaultProgramDirectory
            ? AppContext.BaseDirectory
            : userHomeDirectory!;

        MetadataDirectory = Path.Combine(resolvedHomeDirectory, ".asmroner-data");
        DefaultConfigFilePath = Path.Combine(programDirectory, "config.json");
        DatabaseFilePath = Path.Combine(MetadataDirectory, "asmroner.db");
        DefaultSyncDataDirectory = Path.Combine(MetadataDirectory, "sync-data");
        LogsDirectory = Path.Combine(MetadataDirectory, "logs");
    }

    public string MetadataDirectory { get; }

    public string DefaultConfigFilePath { get; }

    public string DatabaseFilePath { get; }

    public string DefaultSyncDataDirectory { get; }

    public string LogsDirectory { get; }

    public void EnsureMetadataDirectory()
    {
        Directory.CreateDirectory(MetadataDirectory);
    }
}