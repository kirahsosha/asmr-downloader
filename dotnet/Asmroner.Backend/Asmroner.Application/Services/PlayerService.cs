using System.IO;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Library;
using Asmroner.Core.Playback;

namespace Asmroner.Application.Services;

public sealed class PlayerService : IPlayerService
{
    private readonly object _stateLock = new();
    private readonly IMediaLauncher _mediaLauncher;
    private PlaybackContext _context = new()
    {
        Message = "尚未载入任何可播放媒体文件。",
    };

    public PlayerService(IMediaLauncher mediaLauncher)
    {
        _mediaLauncher = mediaLauncher;
    }

    public event EventHandler? ContextChanged;

    public PlaybackContext GetCurrentContext()
    {
        lock (_stateLock)
        {
            return _context;
        }
    }

    public void LoadContext(LibraryWorkItem work, LibraryFileItem file)
    {
        ArgumentNullException.ThrowIfNull(work);
        ArgumentNullException.ThrowIfNull(file);

        if (file.IsDirectory || !file.IsPlayable)
        {
            UpdateContext(new PlaybackContext
            {
                Work = work,
                State = PlaybackState.None,
                Message = "请先在文件树中选择一个可播放的媒体文件。",
            });
            return;
        }

        if (!File.Exists(file.FullPath))
        {
            UpdateContext(new PlaybackContext
            {
                Work = work,
                File = file,
                State = PlaybackState.Failed,
                Message = $"本地媒体文件不存在：{file.RelativePath}",
            });
            return;
        }

        UpdateContext(new PlaybackContext
        {
            Work = work,
            File = file,
            State = PlaybackState.Ready,
            Message = "已载入可播放媒体文件，可通过系统默认程序打开。",
        });
    }

    public void Play()
    {
        var current = GetCurrentContext();
        if (current.File is null)
        {
            UpdateContext(new PlaybackContext
            {
                Work = current.Work,
                State = current.State,
                Message = "请先载入一个可播放的媒体文件。",
            });
            return;
        }

        if (current.State == PlaybackState.Failed)
        {
            UpdateContext(CreateUpdatedContext(current, PlaybackState.Failed, "请先重新载入文件后再打开。"));
            return;
        }

        if (!File.Exists(current.File.FullPath))
        {
            UpdateContext(CreateUpdatedContext(current, PlaybackState.Failed, $"本地媒体文件不存在：{current.File.RelativePath}"));
            return;
        }

        try
        {
            _mediaLauncher.Open(current.File.FullPath);
        }
        catch (Exception ex)
        {
            UpdateContext(CreateUpdatedContext(current, PlaybackState.Failed, $"调用系统默认程序打开失败：{ex.Message}"));
            return;
        }

        UpdateContext(CreateUpdatedContext(current, PlaybackState.Launched, "已调用系统默认程序打开当前媒体文件。"));
    }

    public void ClearContext()
    {
        UpdateContext(new PlaybackContext
        {
            Message = "尚未载入任何可播放媒体文件。",
        });
    }

    private void UpdateContext(PlaybackContext nextContext)
    {
        EventHandler? handler;

        lock (_stateLock)
        {
            _context = nextContext;
            handler = ContextChanged;
        }

        handler?.Invoke(this, EventArgs.Empty);
    }

    private static PlaybackContext CreateUpdatedContext(
        PlaybackContext current,
        PlaybackState state,
        string message)
    {
        return new PlaybackContext
        {
            Work = current.Work,
            File = current.File,
            State = state,
            Message = message,
        };
    }
}