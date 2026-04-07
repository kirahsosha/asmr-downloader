namespace Asmroner.Core.Interfaces;

public interface IAppPathService
{
    string MetadataDirectory { get; }

    string DefaultConfigFilePath { get; }

    string DatabaseFilePath { get; }

    string DefaultDownloadDataDirectory { get; }

    string DefaultSyncDataDirectory { get; }

    string LogsDirectory { get; }

    void EnsureMetadataDirectory();
}