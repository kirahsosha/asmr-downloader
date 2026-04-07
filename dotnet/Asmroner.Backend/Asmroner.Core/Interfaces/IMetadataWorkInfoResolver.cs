using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IMetadataWorkInfoResolver
{
    Task<MetadataWorkInfoResolutionResult> ResolveAsync(IReadOnlyCollection<WorkInfoResolutionRequest> requests, CancellationToken cancellationToken = default);
}

public sealed class WorkInfoResolutionRequest
{
    public string SourceId { get; init; } = string.Empty;

    public int? WorkId { get; init; }
}

public sealed class MetadataWorkInfoResolutionResult
{
    public IReadOnlyDictionary<string, WorkInfoDto> WorkInfos { get; init; } = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<string> FailedSourceIds { get; init; } = Array.Empty<string>();
}