using Asmroner.Core.Library;
using Asmroner.Core.Playback;

namespace Asmroner.Wpf.Views;

public static class LibraryPlaybackSelectionPolicy
{
    public static bool ShouldReloadContext(
        PlaybackContext current,
        LibraryWorkItem? selectedWork,
        LibraryFileItem? selectedFile)
    {
        if (selectedWork is null || selectedFile is null)
        {
            return false;
        }

        if (current.Work is null || current.File is null || current.State == PlaybackState.Failed)
        {
            return true;
        }

        if (!string.Equals(current.Work.SourceId, selectedWork.SourceId, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return !string.Equals(current.File.FullPath, selectedFile.FullPath, StringComparison.OrdinalIgnoreCase);
    }
}