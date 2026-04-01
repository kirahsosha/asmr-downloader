using Asmroner.Core.Configuration;
using Asmroner.Core.Utils;
using System.Globalization;

namespace Asmroner.Wpf.ViewModels;

public static class SearchWorkPageUrlPolicy
{
    private const string SourcePlaceholder = "{RJID}";
    private const string WorkIdPlaceholder = "{WorkId}";

    public static bool TryBuild(string? template, string? sourceId, int workId, out string url, out string errorMessage)
    {
        url = string.Empty;

        var normalizedSourceId = SourceIdNormalizer.Normalize(sourceId);
        var routeToken = ResolveRouteToken(normalizedSourceId, workId);
        if (string.IsNullOrWhiteSpace(routeToken))
        {
            errorMessage = "选中项缺少有效作品编号，无法打开作品页面。";
            return false;
        }

        var normalizedTemplate = string.IsNullOrWhiteSpace(template)
            ? new DownloaderOptions().WorkPageUrlTemplate
            : template.Trim();

        if (normalizedTemplate.Contains(WorkIdPlaceholder, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTemplate = normalizedTemplate.Replace(
                WorkIdPlaceholder,
                workId > 0 ? workId.ToString(CultureInfo.InvariantCulture) : routeToken,
                StringComparison.OrdinalIgnoreCase);
        }

        if (normalizedTemplate.Contains(SourcePlaceholder, StringComparison.OrdinalIgnoreCase))
        {
            url = normalizedTemplate.Replace(SourcePlaceholder, routeToken, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            var prefix = normalizedTemplate.TrimEnd('/');
            url = string.Concat(prefix, "/", routeToken);
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            url = string.Empty;
            errorMessage = "作品页面地址模板无效，请在设置中检查 workPageUrlTemplate。";
            return false;
        }

        url = uri.ToString();
        errorMessage = string.Empty;
        return true;
    }

    private static string ResolveRouteToken(string normalizedSourceId, int workId)
    {
        if (workId > 0 && !normalizedSourceId.StartsWith("RJ", StringComparison.OrdinalIgnoreCase))
        {
            return workId.ToString(CultureInfo.InvariantCulture);
        }

        return normalizedSourceId;
    }
}
