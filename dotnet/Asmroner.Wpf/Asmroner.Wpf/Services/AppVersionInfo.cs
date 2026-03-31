using Asmroner.Core.Constants;

namespace Asmroner.Wpf.Services;

public static class AppVersionInfo
{
    public static string GetDisplayVersion()
    {
        var version = typeof(AppVersionInfo).Assembly.GetName().Version;
        if (version is null)
        {
            return "0.0.0";
        }

        return $"{version.Major}.{version.Minor}.{Math.Max(version.Build, 0)}";
    }

    public static string BuildSettingsVersionText()
    {
        return $"{AsmronerConstants.Application.SettingsVersionPrefix}{GetDisplayVersion()}";
    }

    public static string BuildStartupCompletedMessage()
    {
        return $"{AsmronerConstants.Application.Name} v{GetDisplayVersion()} startup completed.";
    }
}