using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IEnqueueWorkInfoResolver
{
    Task<EnqueueWorkInfoResolutionResult> ResolvePreferTranslatedAsync(IReadOnlyCollection<EnqueueWorkInfoRequest> requests, CancellationToken cancellationToken = default);
}

public sealed class EnqueueWorkInfoRequest
{
    public string SourceId { get; init; } = string.Empty;

    public int? WorkId { get; init; }
}

public sealed class EnqueueWorkInfoResolutionResult
{
    public IReadOnlyDictionary<string, WorkInfoDto> WorkInfos { get; init; } = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<string> FailedSourceIds { get; init; } = Array.Empty<string>();

    public int SwitchedSourceCount { get; init; }
}