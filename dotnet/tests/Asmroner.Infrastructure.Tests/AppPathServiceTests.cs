using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class AppPathServiceTests
{
    [Fact]
    public void LogsDirectory_ShouldBeUnderMetadataDirectory()
    {
        var sut = new AppPathService();

        Assert.StartsWith(sut.MetadataDirectory, sut.LogsDirectory, StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith("logs", sut.LogsDirectory, StringComparison.OrdinalIgnoreCase);
    }
}
