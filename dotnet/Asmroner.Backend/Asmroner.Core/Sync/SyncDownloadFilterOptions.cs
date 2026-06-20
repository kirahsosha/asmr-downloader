using Asmroner.Core.Configuration;

namespace Asmroner.Core.Sync;

/// <summary>
/// 同步下载筛选选项，控制是否使用 Search 高级筛选和/或 Download 文件筛选。
/// </summary>
public sealed record SyncDownloadFilterOptions
{
    /// <summary>
    /// 是否使用 Search 页面的高级筛选条件过滤待同步作品。
    /// 当为 true 时，<see cref="AllowedSourceIds"/> 必须包含已通过 Search 筛选的 SourceId 集合。
    /// </summary>
    public bool UseSearchAdvancedFilters { get; init; }

    /// <summary>
    /// 通过 Search 高级筛选匹配的 SourceId 集合。
    /// 仅当 <see cref="UseSearchAdvancedFilters"/> 为 true 时生效。
    /// 集合为 null 或空时表示无作品匹配。
    /// </summary>
    public IReadOnlySet<string>? AllowedSourceIds { get; init; }

    /// <summary>
    /// 是否使用 Download 页面的文件筛选条件（文件筛选标签 + 高清音频）过滤文件。
    /// </summary>
    public bool UseDownloadFileFilters { get; init; }

    /// <summary>
    /// Download 页面的文件筛选状态。仅当 <see cref="UseDownloadFileFilters"/> 为 true 时生效。
    /// </summary>
    public DownloadUiState? DownloadFilters { get; init; }
}
