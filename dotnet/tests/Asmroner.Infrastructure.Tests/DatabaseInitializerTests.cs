using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class DatabaseInitializerTests
{
    [Fact]
    public async Task DatabaseInitializer_ShouldCreateDatabaseFileAndConnect()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var sut = new DatabaseInitializer(pathService);

            await sut.InitializeAsync();
            var canConnect = await sut.CanConnectAsync();

            Assert.True(File.Exists(pathService.DatabaseFilePath));
            Assert.True(canConnect);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "asmroner-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void CleanupTempRoot(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
}
