using Asmroner.Core.Library;
using Asmroner.Core.Playback;

namespace Asmroner.Core.Interfaces;

public interface IPlayerService
{
    PlaybackContext GetCurrentContext();

    void LoadContext(LibraryWorkItem work, LibraryFileItem? file = null);

    void ClearContext();
}