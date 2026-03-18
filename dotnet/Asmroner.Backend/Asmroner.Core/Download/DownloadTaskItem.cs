using System.ComponentModel.DataAnnotations;

namespace Asmroner.Core.Download;

public enum DownloadTaskStatus
{
    [Display(Name = "未下载")]
    [Order(3)]
    Pending = 0,

    [Display(Name = "待下载")]
    [Order(2)]
    Queued = 1,

    [Display(Name = "下载中")]
    [Order(1)]
    Running = 2,

    [Display(Name = "已完成")]
    [Order(0)]
    Completed = 3,

    [Display(Name = "已失败")]
    [Order(4)]
    Failed = 4,

    [Display(Name = "已取消")]
    [Order(5)]
    Canceled = 5,
}

public sealed class DownloadTaskItem
{
    public Guid TaskId { get; init; } = Guid.NewGuid();

    public string SourceId { get; init; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public DownloadTaskStatus Status { get; set; } = DownloadTaskStatus.Queued;

    public int TotalFiles { get; set; }

    public int CompletedFiles { get; set; }

    public string CurrentFile { get; set; } = string.Empty;

    public double ProgressPercent { get; set; }

    public int RetryCount { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? FinishedAt { get; set; }

    public string TargetDirectory { get; set; } = string.Empty;
}
