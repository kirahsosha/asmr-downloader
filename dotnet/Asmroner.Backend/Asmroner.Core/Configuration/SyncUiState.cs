namespace Asmroner.Core.Configuration;

public sealed class SyncUiState
{
    /// <summary>
    /// 同步下载时使用 Search 页面的高级筛选条件过滤作品。
    /// </summary>
    public bool UseSearchAdvancedFilters { get; set; }

    /// <summary>
    /// 同步下载时使用 Download 页面的文件筛选条件（文件筛选标签 + 高清音频 + 翻译作品）过滤文件。
    /// </summary>
    public bool UseDownloadFileFilters { get; set; }
}
