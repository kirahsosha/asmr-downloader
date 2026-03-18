using Asmroner.Core.Configuration;

namespace Asmroner.Core.Interfaces;

public interface IConfigurationService
{
    Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default);

    IReadOnlyList<string> Validate(AppConfig config);

    bool Exists();
}