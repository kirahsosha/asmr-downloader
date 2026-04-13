using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadToolbarAvailabilityTests
{
    [Fact]
    public void Evaluate_ShouldEnableToolbarActions_WhenNoQueueMutationIsRunning()
    {
        var availability = DownloadToolbarAvailability.Evaluate(isQueueMutationRunning: false);

        Assert.True(availability.CanAddSingle);
        Assert.True(availability.CanAddBatch);
        Assert.True(availability.CanImportFile);
        Assert.True(availability.CanImportFavorites);
        Assert.True(availability.CanRunQueue);
        Assert.True(availability.CanOpenDownloadDirectory);
        Assert.True(availability.CanClearTaskList);
    }

    [Fact]
    public void Evaluate_ShouldKeepOpenDirectoryEnabled_WhenQueueMutationIsRunning()
    {
        var availability = DownloadToolbarAvailability.Evaluate(isQueueMutationRunning: true);

        Assert.False(availability.CanAddSingle);
        Assert.False(availability.CanAddBatch);
        Assert.False(availability.CanImportFile);
        Assert.False(availability.CanImportFavorites);
        Assert.False(availability.CanRunQueue);
        Assert.True(availability.CanOpenDownloadDirectory);
        Assert.False(availability.CanClearTaskList);
    }
}