using System;
namespace Backend.Asmoner.Models
{
    public class MetadataWork
    {
        public int ID { get; set; }
        public string? Title { get; set; }
        public int CircleID { get; set; }
        public string? Name { get; set; }
        public bool Nsfw { get; set; }
        public string? Release { get; set; }
        public int DlCount { get; set; }
        public int Price { get; set; }
        public int ReviewCount { get; set; }
        public int RateCount { get; set; }
        public double RateAverage2Dp { get; set; }
        public bool HasSubtitle { get; set; }
        public string? CreateDate { get; set; }
        public string? Vas { get; set; }
        public string? Tags { get; set; }
        public int Duration { get; set; }
        public string? SourceType { get; set; }
        public string? SourceID { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
