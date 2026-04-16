using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Library;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Services;

public sealed class LibraryScannerService : ILibraryScannerService
{
    private static readonly HashSet<string> PlayableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp3",
        ".wav",
        ".flac",
        ".m4a",
        ".ogg",
        ".aac",
        ".opus",
        ".wma",
        ".webm",
    };

    private readonly IConfigurationService _configurationService;
    private readonly IMetadataSyncStore _metadataSyncStore;
    private readonly Func<string, bool> _directoryExists;
    private readonly Func<string, IReadOnlyList<string>> _getDirectories;
    private readonly Func<string, IReadOnlyList<string>> _getFiles;
    private readonly Func<string, long> _getFileSize;

    public LibraryScannerService(IConfigurationService configurationService, IMetadataSyncStore metadataSyncStore)
        : this(
            configurationService,
            metadataSyncStore,
            Directory.Exists,
            static path => Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly),
            static path => Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly),
            static path => new FileInfo(path).Length)
    {
    }

    public LibraryScannerService(
        IConfigurationService configurationService,
        IMetadataSyncStore metadataSyncStore,
        Func<string, bool> directoryExists,
        Func<string, IReadOnlyList<string>> getDirectories,
        Func<string, IReadOnlyList<string>> getFiles,
        Func<string, long> getFileSize)
    {
        _configurationService = configurationService;
        _metadataSyncStore = metadataSyncStore;
        _directoryExists = directoryExists ?? throw new ArgumentNullException(nameof(directoryExists));
        _getDirectories = getDirectories ?? throw new ArgumentNullException(nameof(getDirectories));
        _getFiles = getFiles ?? throw new ArgumentNullException(nameof(getFiles));
        _getFileSize = getFileSize ?? throw new ArgumentNullException(nameof(getFileSize));
    }

    public async Task<LibraryScanResult> ScanAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var roots = BuildRoots(config);
        var metadataItems = await _metadataSyncStore.GetAllMetadataWorksAsync(cancellationToken);
        var metadataBySourceId = metadataItems
            .Where(static item => !string.IsNullOrWhiteSpace(item.SourceId))
            .GroupBy(static item => item.SourceId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                static group => group.Key,
                static group => group
                    .OrderByDescending(static item => item.UpdatedAt)
                    .ThenByDescending(static item => item.Id)
                    .First(),
                StringComparer.OrdinalIgnoreCase);

        return await Task.Run(
            () => ScanRoots(roots, metadataBySourceId, cancellationToken),
            cancellationToken);
    }

    private LibraryScanResult ScanRoots(
        IReadOnlyList<string> roots,
        IReadOnlyDictionary<string, MetadataWorkItem> metadataBySourceId,
        CancellationToken cancellationToken)
    {

        var mergedWorks = new Dictionary<string, LibraryWorkItem>(StringComparer.OrdinalIgnoreCase);
        var skippedDirectories = new List<string>();
        var errors = new List<string>();
        var scannedRootCount = 0;

        foreach (var root in roots)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_directoryExists(root))
            {
                errors.Add($"资源目录不存在：{root}");
                continue;
            }

            scannedRootCount++;

            IReadOnlyList<string> directories;
            try
            {
                directories = _getDirectories(root);
            }
            catch (Exception ex)
            {
                errors.Add($"读取资源目录失败：{root} ({ex.Message})");
                continue;
            }

            foreach (var workDirectory in directories.OrderBy(static path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var directoryName = Path.GetFileName(workDirectory);
                if (!LibraryDirectoryNameParser.TryParse(directoryName, out var parsed))
                {
                    skippedDirectories.Add(workDirectory);
                    continue;
                }

                try
                {
                    metadataBySourceId.TryGetValue(parsed.SourceId, out var metadataItem);
                    var files = BuildFileTree(workDirectory, workDirectory, errors);
                    var workItem = CreateWorkItem(root, workDirectory, parsed, metadataItem, files);

                    if (!mergedWorks.TryGetValue(workItem.SourceId, out var existingWork)
                        || ShouldReplace(existingWork, workItem))
                    {
                        mergedWorks[workItem.SourceId] = workItem;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"扫描目录失败：{workDirectory} ({ex.Message})");
                }
            }
        }

        var items = mergedWorks.Values
            .OrderByDescending(static item => ParseRelease(item.Release))
            .ThenBy(static item => item.SourceId, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new LibraryScanResult
        {
            Items = items,
            SkippedDirectories = skippedDirectories
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            Errors = errors
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static error => error, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            ScannedRootCount = scannedRootCount,
        };
    }

    private static IReadOnlyList<string> BuildRoots(AppConfig config)
    {
        return new[]
            {
                config.Downloader.DownloadDataFolder,
                config.Downloader.SyncDataFolder,
            }
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .Select(static path => Path.GetFullPath(path.Trim()))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static LibraryWorkItem CreateWorkItem(
        string sourceRoot,
        string workDirectory,
        LibraryDirectoryNameParseResult parsed,
        MetadataWorkItem? metadataItem,
        IReadOnlyList<LibraryFileItem> files)
    {
        return new LibraryWorkItem
        {
            WorkId = metadataItem?.Id ?? 0,
            SourceId = parsed.SourceId,
            Title = string.IsNullOrWhiteSpace(metadataItem?.Title)
                ? parsed.Title
                : metadataItem.Title,
            Release = string.IsNullOrWhiteSpace(metadataItem?.Release)
                ? parsed.Release
                : metadataItem.Release,
            HasSubtitle = metadataItem?.HasSubtitle ?? parsed.HasSubtitle ?? false,
            Tags = metadataItem?.Tags ?? string.Empty,
            RootDirectory = workDirectory,
            SourceRoot = sourceRoot,
            DirectoryScheme = parsed.Scheme,
            TotalFileCount = CountFiles(files),
            AudioFileCount = CountPlayableFiles(files),
            Files = files,
        };
    }

    private static bool ShouldReplace(LibraryWorkItem current, LibraryWorkItem candidate)
    {
        if (candidate.AudioFileCount != current.AudioFileCount)
        {
            return candidate.AudioFileCount > current.AudioFileCount;
        }

        if (candidate.TotalFileCount != current.TotalFileCount)
        {
            return candidate.TotalFileCount > current.TotalFileCount;
        }

        return false;
    }

    private IReadOnlyList<LibraryFileItem> BuildFileTree(
        string currentDirectory,
        string rootDirectory,
        ICollection<string> errors)
    {
        var items = new List<LibraryFileItem>();

        IReadOnlyList<string> directories = Array.Empty<string>();
        try
        {
            directories = _getDirectories(currentDirectory);
        }
        catch (Exception ex)
        {
            errors.Add($"读取目录失败：{BuildTreeLocation(rootDirectory, currentDirectory)} ({ex.Message})");
        }

        foreach (var directory in directories.OrderBy(static path => path, StringComparer.OrdinalIgnoreCase))
        {
            items.Add(new LibraryFileItem
            {
                Name = Path.GetFileName(directory),
                RelativePath = NormalizeRelativePath(rootDirectory, directory),
                FullPath = directory,
                IsDirectory = true,
                Children = BuildFileTree(directory, rootDirectory, errors),
            });
        }

        IReadOnlyList<string> files = Array.Empty<string>();
        try
        {
            files = _getFiles(currentDirectory);
        }
        catch (Exception ex)
        {
            errors.Add($"读取目录文件失败：{BuildTreeLocation(rootDirectory, currentDirectory)} ({ex.Message})");
        }

        foreach (var file in files.OrderBy(static path => path, StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                var extension = Path.GetExtension(file);
                items.Add(new LibraryFileItem
                {
                    Name = Path.GetFileName(file),
                    RelativePath = NormalizeRelativePath(rootDirectory, file),
                    FullPath = file,
                    Extension = extension,
                    IsDirectory = false,
                    IsPlayable = PlayableExtensions.Contains(extension),
                    SizeBytes = _getFileSize(file),
                });
            }
            catch (Exception ex)
            {
                errors.Add($"读取文件失败：{BuildTreeLocation(rootDirectory, file)} ({ex.Message})");
            }
        }

        return items;
    }

    private static string BuildTreeLocation(string rootDirectory, string targetPath)
    {
        var relativePath = NormalizeRelativePath(rootDirectory, targetPath);
        return string.Equals(relativePath, ".", StringComparison.Ordinal)
            ? Path.GetFileName(targetPath)
            : relativePath;
    }

    private static string NormalizeRelativePath(string rootDirectory, string targetPath)
    {
        return Path.GetRelativePath(rootDirectory, targetPath).Replace('\\', '/');
    }

    private static int CountFiles(IReadOnlyList<LibraryFileItem> items)
    {
        var count = 0;
        foreach (var item in items)
        {
            count += item.IsDirectory ? CountFiles(item.Children) : 1;
        }

        return count;
    }

    private static int CountPlayableFiles(IReadOnlyList<LibraryFileItem> items)
    {
        var count = 0;
        foreach (var item in items)
        {
            count += item.IsDirectory
                ? CountPlayableFiles(item.Children)
                : item.IsPlayable ? 1 : 0;
        }

        return count;
    }

    private static DateTime ParseRelease(string release)
    {
        return DateTime.TryParse(release, out var parsed)
            ? parsed
            : DateTime.MinValue;
    }
}