using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backend.Asmoner.Services
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

    public class FolderScanner
    {
        private readonly string _baseDir;

        public FolderScanner()
        {
            // default to syncdata under repo root if exists, else current directory
            var candidate = Path.Combine(Directory.GetCurrentDirectory(), "syncdata");
            if (Directory.Exists(candidate)) _baseDir = candidate;
            else _baseDir = Directory.GetCurrentDirectory();
        }

        public (List<FolderInfo>, int) GetPage(int page, int pageSize)
        {
            var list = new List<FolderInfo>();
            if (!Directory.Exists(_baseDir)) return (list, 0);

            var entries = Directory.GetDirectories(_baseDir);
            var total = entries.Length;
            var pageEntries = entries.Skip((page - 1) * pageSize).Take(pageSize);

            long idCounter = 1;
            foreach (var entry in pageEntries)
            {
                var name = Path.GetFileName(entry);
                var fi = new FolderInfo
                {
                    Id = idCounter++,
                    Name = name,
                    MediaId = ExtractMediaId(name),
                    Date = ExtractDate(name),
                    HasSubtitles = name.Contains("-sub-") || name.Contains("-sub-"),
                    Title = ExtractTitle(name),
                    BaseDir = Path.GetFileName(_baseDir),
                    Files = ScanFiles(entry)
                };
                list.Add(fi);
            }
            return (list, total);
        }

        private List<FileInfoModel> ScanFiles(string folder)
        {
            var res = new List<FileInfoModel>();
            var files = Directory.EnumerateFileSystemEntries(folder, "*", SearchOption.AllDirectories);
            long id = 1;
            foreach (var f in files)
            {
                var name = Path.GetFileName(f);
                var isDir = Directory.Exists(f);
                res.Add(new FileInfoModel { Id = id++, FolderId = 0, Path = Path.GetRelativePath(_baseDir, f).Replace('\\', '/'), Name = name, IsDir = isDir });
            }
            return res;
        }

        private string? ExtractMediaId(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var parts = name.Split('-');
            if (parts.Length > 0) return parts[0];
            return null;
        }

        private string? ExtractDate(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var parts = name.Split('-');
            if (parts.Length > 1) return parts[1];
            return null;
        }

        private string? ExtractTitle(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var parts = name.Split('-');
            if (parts.Length >= 4) return parts[3];
            return name;
        }
    }
}
