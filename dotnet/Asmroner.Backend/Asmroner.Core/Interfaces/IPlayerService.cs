using Asmroner.Core.Library;
using Asmroner.Core.Playback;

namespace Asmroner.Core.Interfaces;

public interface IPlayerService
{
    event EventHandler? ContextChanged;

    PlaybackContext GetCurrentContext();

    void LoadContext(LibraryWorkItem work, LibraryFileItem file);

    void Play();

    void ClearContext();
}