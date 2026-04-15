using Asmroner.Core.Interfaces;
using Asmroner.Core.Library;

namespace Asmroner.Application.Services;

public sealed class LibraryQueryService : ILibraryQueryService
{
    private readonly ILibraryScannerService _libraryScannerService;

    public LibraryQueryService(ILibraryScannerService libraryScannerService)
    {
        _libraryScannerService = libraryScannerService;
    }

    public async Task<LibraryQueryResult> QueryAsync(LibraryQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var scanResult = await _libraryScannerService.ScanAsync(cancellationToken);
        var keyword = query.Keyword.Trim();
        var pageSize = Math.Max(1, query.PageSize);

        var filtered = scanResult.Items
            .Where(item => MatchesKeyword(item, keyword))
            .Where(item => !query.SubtitleOnly || item.HasSubtitle)
            .Where(item => !query.AudioOnly || item.AudioFileCount > 0)
            .OrderByDescending(static item => ParseRelease(item.Release))
            .ThenBy(static item => item.SourceId, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var totalCount = filtered.Length;
        var totalPages = totalCount == 0
            ? 1
            : (int)Math.Ceiling(totalCount / (double)pageSize);
        var page = Math.Clamp(query.Page, 1, totalPages);
        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return new LibraryQueryResult
        {
            Items = items,
            SkippedDirectories = scanResult.SkippedDirectories,
            Errors = scanResult.Errors,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            ScannedRootCount = scanResult.ScannedRootCount,
            ScannedWorkCount = scanResult.Items.Count,
        };
    }

    private static bool MatchesKeyword(LibraryWorkItem item, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return true;
        }

        return Contains(item.SourceId, keyword)
            || Contains(item.Title, keyword)
            || Contains(item.Release, keyword)
            || Contains(item.Tags, keyword)
            || Contains(item.RootDirectory, keyword);
    }

    private static bool Contains(string value, string keyword)
    {
        return value.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    private static DateTime ParseRelease(string release)
    {
        return DateTime.TryParse(release, out var parsed)
            ? parsed
            : DateTime.MinValue;
    }
}