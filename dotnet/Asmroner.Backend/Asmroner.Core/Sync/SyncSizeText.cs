using System.Globalization;
using System.Text.RegularExpressions;

namespace Asmroner.Core.Sync;

public static partial class SyncSizeText
{
    private static readonly IReadOnlyDictionary<string, long> Units = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
    {
        ["B"] = 1L,
        ["KB"] = 1024L,
        ["MB"] = 1024L * 1024L,
        ["GB"] = 1024L * 1024L * 1024L,
        ["TB"] = 1024L * 1024L * 1024L * 1024L,
    };

    public static long ParseBytes(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new ArgumentException("同步容量上限不能为空。", nameof(raw));
        }

        var match = SizePattern().Match(raw.Trim());
        if (!match.Success)
        {
            throw new ArgumentException("同步容量上限格式无效，应为 5GB、1024MB 这类写法。", nameof(raw));
        }

        var number = decimal.Parse(match.Groups["value"].Value, CultureInfo.InvariantCulture);
        if (number <= 0)
        {
            throw new ArgumentException("同步容量上限必须大于 0。", nameof(raw));
        }

        var unit = match.Groups["unit"].Success
            ? match.Groups["unit"].Value.ToUpperInvariant()
            : "B";

        if (!Units.TryGetValue(unit, out var multiplier))
        {
            throw new ArgumentException("同步容量上限单位无效，仅支持 B/KB/MB/GB/TB。", nameof(raw));
        }

        return checked((long)Math.Ceiling(number * multiplier));
    }

    public static string FormatBytes(long bytes)
    {
        if (bytes < 0)
        {
            return $"-{FormatBytes(Math.Abs(bytes))}";
        }

        if (bytes < 1024)
        {
            return $"{bytes.ToString(CultureInfo.InvariantCulture)} B";
        }

        var units = new[] { "KB", "MB", "GB", "TB" };
        var value = (decimal)bytes;
        var unitIndex = -1;
        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        var format = value >= 100 ? "0" : "0.##";
        return string.Create(CultureInfo.InvariantCulture, $"{value.ToString(format, CultureInfo.InvariantCulture)} {units[unitIndex]}");
    }

    [GeneratedRegex(@"^(?<value>\d+(?:\.\d+)?)\s*(?<unit>B|KB|MB|GB|TB)?$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SizePattern();
}