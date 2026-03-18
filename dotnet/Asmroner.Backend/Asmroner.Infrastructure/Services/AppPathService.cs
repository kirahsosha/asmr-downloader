using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class AppPathService : IAppPathService
{
    public AppPathService(string? userHomeDirectory = null)
    {
        var homeDirectory = userHomeDirectory;
        if (string.IsNullOrWhiteSpace(homeDirectory))
        {
            homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        MetadataDirectory = Path.Combine(homeDirectory, ".asmroner-data");
        ConfigFilePath = Path.Combine(MetadataDirectory, "config.toml");
        DatabaseFilePath = Path.Combine(MetadataDirectory, "asmroner.db");
        DefaultSyncDataDirectory = Path.Combine(MetadataDirectory, "sync-data");
    }

    public string MetadataDirectory { get; }

    public string ConfigFilePath { get; }

    public string DatabaseFilePath { get; }

    public string DefaultSyncDataDirectory { get; }

    public void EnsureMetadataDirectory()
    {
        Directory.CreateDirectory(MetadataDirectory);
    }
}