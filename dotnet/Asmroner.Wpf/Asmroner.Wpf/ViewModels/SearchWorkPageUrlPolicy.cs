using Asmroner.Core.Configuration;
using Asmroner.Core.Utils;

namespace Asmroner.Wpf.ViewModels;

public static class SearchWorkPageUrlPolicy
{
    private const string SourcePlaceholder = "{RJID}";

    public static bool TryBuild(string? template, string? sourceId, out string url, out string errorMessage)
    {
        url = string.Empty;

        var normalizedSourceId = SourceIdNormalizer.Normalize(sourceId);
        if (string.IsNullOrWhiteSpace(normalizedSourceId))
        {
            errorMessage = "选中项缺少有效 RJID，无法打开作品页面。";
            return false;
        }

        var normalizedTemplate = string.IsNullOrWhiteSpace(template)
            ? new DownloaderOptions().WorkPageUrlTemplate
            : template.Trim();

        if (normalizedTemplate.Contains(SourcePlaceholder, StringComparison.OrdinalIgnoreCase))
        {
            url = normalizedTemplate.Replace(SourcePlaceholder, normalizedSourceId, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            var prefix = normalizedTemplate.TrimEnd('/');
            url = string.Concat(prefix, "/", normalizedSourceId);
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
}
