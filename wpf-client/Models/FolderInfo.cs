using System.Collections.Generic;

namespace AsmrDownloader.Client.Models
{
    public class FileInfoModel
    {
        public long Id { get; set; }
        public long FolderId { get; set; }
        public string? Path { get; set; }
        public string? Name { get; set; }
        public bool IsDir { get; set; }
    }

    public class FolderInfo
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? MediaId { get; set; }
        public string? Date { get; set; }
        public bool HasSubtitles { get; set; }
        public string? Title { get; set; }
        public string? BaseDir { get; set; }
        public List<FileInfoModel>? Files { get; set; }
    }
}
