namespace Asmroner.Wpf.Tests;

internal static class XamlTestPathLocator
{
    public static string Locate(string fileName, params string[] pathSegments)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null)
        {
            var combined = pathSegments.Concat(new[] { fileName }).ToArray();
            var candidate = Path.Combine(new[] { root.FullName }.Concat(combined).ToArray());
            if (File.Exists(candidate))
            {
                return candidate;
            }

            root = root.Parent;
        }

        throw new FileNotFoundException($"Cannot locate {fileName} from test base directory.");
    }
}
