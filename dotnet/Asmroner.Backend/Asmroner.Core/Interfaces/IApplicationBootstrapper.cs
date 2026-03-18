using Asmroner.Core.Initialization;

namespace Asmroner.Core.Interfaces;

public interface IApplicationBootstrapper
{
    Task<BootstrapResult> InitializeAsync(CancellationToken cancellationToken = default);
}