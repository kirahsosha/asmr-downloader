using Asmroner.Core.Library;

namespace Asmroner.Core.Playback;

public enum PlaybackState
{
    None = 0,
    Ready = 1,
    Launched = 2,
    Failed = 3,
}

public sealed class PlaybackContext
{
    public LibraryWorkItem? Work { get; init; }

    public LibraryFileItem? File { get; init; }

    public PlaybackState State { get; init; } = PlaybackState.None;

    public string Message { get; init; } = string.Empty;
}