using System.Globalization;
using System.Text;
using System.Text.Json;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Services;

public sealed class SyncExportService : ISyncExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private readonly IMetadataSyncStore _metadataSyncStore;

    public SyncExportService(IMetadataSyncStore metadataSyncStore)
    {
        _metadataSyncStore = metadataSyncStore;
    }

    public async Task<SyncExportResult> ExportAsync(SyncExportStatus status, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var normalizedFilePath = Path.GetFullPath(filePath);
        var format = ResolveFormat(normalizedFilePath);
        var items = await _metadataSyncStore.GetSyncDownloadsByStatusAsync(MapStatus(status), cancellationToken);

        if (items.Count == 0)
        {
            return new SyncExportResult
            {
                Status = status,
                ExportedCount = 0,
                FilePath = normalizedFilePath,
                Format = format,
                Message = $"没有可导出的{GetStatusLabel(status)}同步记录。",
            };
        }

        if (format == "csv")
        {
            await ExportCsvAsync(items, normalizedFilePath, cancellationToken);
        }
        else
        {
            await ExportJsonAsync(items, normalizedFilePath, cancellationToken);
        }

        return new SyncExportResult
        {
            Status = status,
            ExportedCount = items.Count,
            FilePath = normalizedFilePath,
            Format = format,
            Message = $"已导出 {items.Count.ToString(CultureInfo.InvariantCulture)} 条{GetStatusLabel(status)}同步记录：{normalizedFilePath}",
        };
    }

    private static async Task ExportCsvAsync(IReadOnlyList<WorkSyncInfoItem> items, string filePath, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");

        var lines = new List<string>(items.Count + 1)
        {
            "id,metadata_work_id,source_id,dir_size,status,file_path,updated_at,fail_reason,retry_count,failed_at,has_subtitle",
        };

        foreach (var item in items)
        {
            lines.Add(string.Join(',',
                item.Id.ToString(CultureInfo.InvariantCulture),
                item.MetadataWorkId.ToString(CultureInfo.InvariantCulture),
                EscapeCsv(item.SourceId),
                item.DirSize.ToString(CultureInfo.InvariantCulture),
                EscapeCsv(item.Status),
                EscapeCsv(item.FilePath),
                EscapeCsv(item.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)),
                EscapeCsv(item.FailReason),
                item.RetryCount.ToString(CultureInfo.InvariantCulture),
                EscapeCsv(item.FailedAt?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty),
                item.HasSubtitle ? "true" : "false"));
        }

        await File.WriteAllLinesAsync(filePath, lines, Encoding.UTF8, cancellationToken);
    }

    private static async Task ExportJsonAsync(IReadOnlyList<WorkSyncInfoItem> items, string filePath, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, items, JsonOptions, cancellationToken);
    }

    private static string ResolveFormat(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return "csv";
        }

        if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            return "json";
        }

        throw new ArgumentException("仅支持导出 CSV 或 JSON 文件。", nameof(filePath));
    }

    private static string MapStatus(SyncExportStatus status)
    {
        return status == SyncExportStatus.Failed ? "FAILED" : "COMPLETED";
    }

    private static string GetStatusLabel(SyncExportStatus status)
    {
        return status == SyncExportStatus.Failed ? "失败" : "成功";
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}