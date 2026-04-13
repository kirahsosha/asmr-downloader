namespace Asmroner.Wpf.ViewModels;

public sealed class DownloadToolbarAvailability
{
    public bool CanAddSingle { get; init; }

    public bool CanAddBatch { get; init; }

    public bool CanImportFile { get; init; }

    public bool CanImportFavorites { get; init; }

    public bool CanRunQueue { get; init; }

    public bool CanOpenDownloadDirectory { get; init; }

    public bool CanClearTaskList { get; init; }

    public static DownloadToolbarAvailability Evaluate(bool isQueueMutationRunning)
    {
        var canMutateQueue = !isQueueMutationRunning;

        return new DownloadToolbarAvailability
        {
            CanAddSingle = canMutateQueue,
            CanAddBatch = canMutateQueue,
            CanImportFile = canMutateQueue,
            CanImportFavorites = canMutateQueue,
            CanRunQueue = canMutateQueue,
            CanOpenDownloadDirectory = true,
            CanClearTaskList = canMutateQueue,
        };
    }
}