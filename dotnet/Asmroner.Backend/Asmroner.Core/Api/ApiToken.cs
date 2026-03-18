namespace Asmroner.Core.Api;

public sealed class ApiToken
{
    public string Scheme { get; init; } = "Bearer";

    public required string AccessToken { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }

    public bool IsExpired(DateTimeOffset? now = null)
    {
        if (ExpiresAt is null)
        {
            return false;
        }

        return ExpiresAt <= (now ?? DateTimeOffset.UtcNow);
    }

    public string ToAuthorizationHeader() => $"{Scheme} {AccessToken}";
}