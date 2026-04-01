using Asmroner.Application.Services;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;

namespace Asmroner.Application.Tests;

public class EnqueueWorkInfoResolverTests
{
    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldCreateSelectedEditionWorkInfo_WhenPreferredEditionOnlyExistsInRelatedEditions()
    {
        var apiClient = new ScriptedApiClient(workInfos:
        [
            new WorkInfoDto
            {
                SourceId = "RJ2001",
                Title = "日文原版",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    IsOriginal = true,
                },
                OtherLanguageEditionsInDb =
                [
                    new WorkOtherLanguageEditionDto
                    {
                        Id = 2002,
                        SourceId = "RJ2002",
                        Lang = "简体中文",
                        Title = "简中版",
                    },
                ],
            },
        ]);
        var sut = new EnqueueWorkInfoResolver(apiClient);

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "RJ2001" },
        ]);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("RJ2002", resolved.Key);
        Assert.Equal(2002, resolved.Value.Id);
        Assert.Equal("RJ2002", resolved.Value.SourceId);
        Assert.Equal("简中版", resolved.Value.Title);
        Assert.Empty(result.FailedSourceIds);
        Assert.Equal(1, result.SwitchedSourceCount);
    }

    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldReuseFetchedPreferredEdition_AndDeduplicateFinalSourceIds()
    {
        var apiClient = new ScriptedApiClient(workInfos:
        [
            new WorkInfoDto
            {
                SourceId = "RJ2101",
                Title = "日文原版",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    IsOriginal = true,
                },
                OtherLanguageEditionsInDb =
                [
                    new WorkOtherLanguageEditionDto
                    {
                        SourceId = "RJ2102",
                        Lang = "简体中文",
                        Title = "简中版",
                    },
                ],
            },
            new WorkInfoDto
            {
                SourceId = "RJ2102",
                Title = "简中完整版",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    Lang = "CHI_HANS",
                },
            },
        ]);
        var sut = new EnqueueWorkInfoResolver(apiClient);

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "RJ2101" },
            new EnqueueWorkInfoRequest { SourceId = "RJ2102" },
        ]);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("RJ2102", resolved.Key);
        Assert.Equal("简中完整版", resolved.Value.Title);
        Assert.Empty(result.FailedSourceIds);
        Assert.Equal(1, result.SwitchedSourceCount);
    }

    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldReturnFailedSourceIds_WhenFetchFails()
    {
        var apiClient = new ScriptedApiClient(
            failOnWorkInfoIds: ["RJ2202"],
            workInfos:
            [
                new WorkInfoDto
                {
                    SourceId = "RJ2201",
                    Title = "日文原版",
                    TranslationInfo = new WorkTranslationInfoDto
                    {
                        IsOriginal = true,
                    },
                },
            ]);
        var sut = new EnqueueWorkInfoResolver(apiClient);

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "RJ2201" },
            new EnqueueWorkInfoRequest { SourceId = "RJ2202" },
        ]);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("RJ2201", resolved.Key);
        Assert.Equal(["RJ2202"], result.FailedSourceIds);
        Assert.Equal(0, result.SwitchedSourceCount);
    }

    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldUseWorkIdWhenProvided()
    {
        var apiClient = new ScriptedApiClient(workInfos:
        [
            new WorkInfoDto
            {
                Id = 100000062,
                SourceId = "BJ02370869",
                Title = "BJ original",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    IsOriginal = true,
                },
            },
        ]);
        var sut = new EnqueueWorkInfoResolver(apiClient);

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "BJ02370869", WorkId = 100000062 },
        ]);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("BJ02370869", resolved.Key);
        Assert.Equal(100000062, resolved.Value.Id);
        Assert.Empty(result.FailedSourceIds);
        Assert.Equal(0, result.SwitchedSourceCount);
    }
}