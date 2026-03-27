using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public sealed class DownloadTaskRowViewModel
{
    public Guid TaskId { get; init; }

    public string SourceId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public DownloadTaskStatus Status { get; init; }

    public string StatusText { get; init; } = string.Empty;

    public int StatusSortOrder { get; init; }

    public string ProgressText { get; init; } = "0%";

    public string ErrorMessage { get; init; } = string.Empty;

    public string TargetDirectory { get; init; } = string.Empty;

    public static DownloadTaskRowViewModel CreatePending(string sourceId, string title, DownloadTaskStatus status)
    {
        return new DownloadTaskRowViewModel
        {
            TaskId = Guid.Empty,
            SourceId = sourceId,
            Title = title,
            Status = status,
            StatusText = status.GetDisplayName(),
            StatusSortOrder = status.GetSortOrder(),
            ProgressText = string.Empty,
            ErrorMessage = status == DownloadTaskStatus.Canceled ? "任务已取消。" : string.Empty,
        };
    }

    public static DownloadTaskRowViewModel From(DownloadTaskItem item)
    {
        return new DownloadTaskRowViewModel
        {
            TaskId = item.TaskId,
            SourceId = item.SourceId,
            Title = item.Title,
            Status = item.Status,
            StatusText = item.Status.GetDisplayName(),
            StatusSortOrder = item.Status.GetSortOrder(),
            ProgressText = $"{Math.Round(item.ProgressPercent, 1)}% ({item.CompletedFiles}/{item.TotalFiles})",
            ErrorMessage = item.ErrorMessage,
            TargetDirectory = item.TargetDirectory,
        };
    }
}
