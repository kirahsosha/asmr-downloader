using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface ISyncWorkInfoResolver
{
    Task<IReadOnlyList<WorkInfoDto>> ResolveAllAsync(CancellationToken cancellationToken = default);
}