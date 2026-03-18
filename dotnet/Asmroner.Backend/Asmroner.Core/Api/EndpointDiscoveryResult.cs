namespace Asmroner.Core.Api;

public sealed class EndpointDiscoveryResult
{
    public required string BaseUrl { get; init; }

    public long LatencyMs { get; init; }

    public IReadOnlyList<string> Candidates { get; init; } = Array.Empty<string>();
}