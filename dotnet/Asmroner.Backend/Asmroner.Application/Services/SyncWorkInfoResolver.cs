using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;

namespace Asmroner.Application.Services;

public sealed class SyncWorkInfoResolver : ISyncWorkInfoResolver
{
    private readonly IMetadataSyncStore _metadataSyncStore;

    public SyncWorkInfoResolver(IMetadataSyncStore metadataSyncStore)
    {
        _metadataSyncStore = metadataSyncStore;
    }

    public async Task<IReadOnlyList<WorkInfoDto>> ResolveAllAsync(CancellationToken cancellationToken = default)
    {
        var metadataWorks = await _metadataSyncStore.GetAllMetadataWorksAsync(cancellationToken);
        return metadataWorks
            .OrderBy(static work => work.Id)
            .Select(MetadataWorkSummaryMapper.ToWorkInfoDto)
            .ToArray();
    }
}