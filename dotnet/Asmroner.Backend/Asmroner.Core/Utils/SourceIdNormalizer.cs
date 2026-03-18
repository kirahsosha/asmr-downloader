using System.Text.RegularExpressions;

namespace Asmroner.Core.Utils;

public static class SourceIdNormalizer
{
    private static readonly Regex RjFlexiblePattern = new(@"RJ\D*(\d+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly Regex DigitsOnlyPattern = new(@"^\d+$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>
    /// 归一化为标准 sourceId 格式，例如 "RJ01426915"。
    /// 用于展示、数据库存储、目录命名等场景。
    /// </summary>
    public static string Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        var trimmed = raw.Trim();
        var decoded = Uri.UnescapeDataString(trimmed);

        var rjMatch = RjFlexiblePattern.Match(decoded);
        if (rjMatch.Success)
        {
            return "RJ" + rjMatch.Groups[1].Value;
        }

        if (DigitsOnlyPattern.IsMatch(decoded))
        {
            return "RJ" + decoded;
        }

        return decoded;
    }

    /// <summary>
    /// 提取用于 API 路径的纯数字部分，例如 "RJ01426915" -> "01426915"。
    /// asmr.one API 端点（/api/work/{id}、/api/tracks/{id}）仅接受纯数字 id，
    /// 传入含前缀的 "RJ01426915" 将返回 400 Bad Request。
    /// </summary>
    public static string ToApiNumericId(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        var trimmed = raw.Trim();
        var decoded = Uri.UnescapeDataString(trimmed);

        var rjMatch = RjFlexiblePattern.Match(decoded);
        if (rjMatch.Success)
        {
            return rjMatch.Groups[1].Value;
        }

        if (DigitsOnlyPattern.IsMatch(decoded))
        {
            return decoded;
        }

        return decoded;
    }
}