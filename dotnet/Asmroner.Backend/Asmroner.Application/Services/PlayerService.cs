using Asmroner.Core.Interfaces;
using Asmroner.Core.Library;
using Asmroner.Core.Playback;

namespace Asmroner.Application.Services;

public sealed class PlayerService : IPlayerService
{
    private readonly object _stateLock = new();
    private PlaybackContext _context = new()
    {
        Message = "尚未载入任何本地音频。",
    };

    public PlaybackContext GetCurrentContext()
    {
        lock (_stateLock)
        {
            return _context;
        }
    }

    public void LoadContext(LibraryWorkItem work, LibraryFileItem? file = null)
    {
        ArgumentNullException.ThrowIfNull(work);

        var selectedFile = file ?? FindFirstPlayableFile(work.Files);

        lock (_stateLock)
        {
            _context = new PlaybackContext
            {
                Work = work,
                File = selectedFile,
                State = selectedFile is null ? PlaybackState.None : PlaybackState.Ready,
                Message = selectedFile is null
                    ? "当前作品没有可载入的本地音频文件。"
                    : "已载入播放上下文，播放器将在后续批次接入。",
            };
        }
    }

    public void ClearContext()
    {
        lock (_stateLock)
        {
            _context = new PlaybackContext
            {
                Message = "尚未载入任何本地音频。",
            };
        }
    }

    private static LibraryFileItem? FindFirstPlayableFile(IReadOnlyList<LibraryFileItem> items)
    {
        foreach (var item in items)
        {
            if (item.IsDirectory)
            {
                var nested = FindFirstPlayableFile(item.Children);
                if (nested is not null)
                {
                    return nested;
                }

                continue;
            }

            if (item.IsPlayable)
            {
                return item;
            }
        }

        return null;
    }
}