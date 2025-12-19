using System;
namespace Backend.Asmoner.Models
{
    public class WorkSyncInfo
    {
        public int ID { get; set; }
        public int MetadataWorkId { get; set; }
        public string? SourceId { get; set; }
        public bool HasSubtitle { get; set; }
        public long DirSize { get; set; }
        public string? Status { get; set; }
        public string? FilePath { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? FailReason { get; set; }
        public int RetryCount { get; set; }
        public DateTime? FailedAt { get; set; }
    }
}
