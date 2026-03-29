namespace Asmroner.Core.Interfaces;

public interface IAppPathService
{
    string MetadataDirectory { get; }

    string DefaultConfigFilePath { get; }

    string DatabaseFilePath { get; }

    string DefaultSyncDataDirectory { get; }

    void EnsureMetadataDirectory();
}