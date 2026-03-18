using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IAsmrApiOptionsProvider
{
    Task<AsmrApiOptions> GetOptionsAsync(CancellationToken cancellationToken = default);
}