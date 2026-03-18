namespace Asmroner.Core.Api;

public sealed class AsmrApiOptions
{
    public required string BaseUrl { get; init; }

    public string? ProxyUrl { get; init; }

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

    public IReadOnlyList<string> CandidateBaseUrls { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> PublishSourceUrls { get; init; } = Array.Empty<string>();
}