using Asmroner.Application.Services;
using Asmroner.Core.Api;

namespace Asmroner.Application.Tests;

public class WorkLanguageSelectionPolicyTests
{
    [Fact]
    public void SelectPreferredEdition_ShouldPreferSimplifiedChinese_WhenAvailable()
    {
        var workInfo = new WorkInfoDto
        {
            SourceId = "RJ1001",
            Title = "日文原版",
            TranslationInfo = new WorkTranslationInfoDto
            {
                IsOriginal = true,
            },
            OtherLanguageEditionsInDb =
            [
                new WorkOtherLanguageEditionDto { SourceId = "RJ1003", Lang = "繁体中文", Title = "繁中版" },
                new WorkOtherLanguageEditionDto { SourceId = "RJ1002", Lang = "简体中文", Title = "简中版" },
            ],
        };

        var result = WorkLanguageSelectionPolicy.SelectPreferredEdition(workInfo);

        Assert.Equal(0, result.SelectedWorkId);
        Assert.Equal("RJ1002", result.SelectedSourceId);
        Assert.Equal("简中版", result.SelectedTitle);
        Assert.Equal("简体中文", result.SelectedLanguage);
        Assert.False(result.IsCurrentWork);
    }

    [Fact]
    public void SelectPreferredEdition_ShouldPreferTraditionalChinese_WhenSimplifiedMissing()
    {
        var workInfo = new WorkInfoDto
        {
            SourceId = "RJ1101",
            Title = "日文原版",
            TranslationInfo = new WorkTranslationInfoDto
            {
                IsOriginal = true,
            },
            OtherLanguageEditionsInDb =
            [
                new WorkOtherLanguageEditionDto { SourceId = "RJ1102", Lang = "繁体中文", Title = "繁中版" },
            ],
        };

        var result = WorkLanguageSelectionPolicy.SelectPreferredEdition(workInfo);

        Assert.Equal(0, result.SelectedWorkId);
        Assert.Equal("RJ1102", result.SelectedSourceId);
        Assert.Equal("繁体中文", result.SelectedLanguage);
    }

    [Fact]
    public void SelectPreferredEdition_ShouldKeepJapaneseCurrentWork_WhenNoChineseEditionExists()
    {
        var workInfo = new WorkInfoDto
        {
            SourceId = "RJ1201",
            Title = "日文原版",
            TranslationInfo = new WorkTranslationInfoDto
            {
                IsOriginal = true,
            },
            OtherLanguageEditionsInDb =
            [
                new WorkOtherLanguageEditionDto { SourceId = "RJ1202", Lang = "ENG", Title = "English" },
            ],
        };

        var result = WorkLanguageSelectionPolicy.SelectPreferredEdition(workInfo);

        Assert.Equal(0, result.SelectedWorkId);
        Assert.Equal("RJ1201", result.SelectedSourceId);
        Assert.Equal("日本語", result.SelectedLanguage);
        Assert.True(result.IsCurrentWork);
    }

    [Fact]
    public void ResolveCurrentWorkLanguage_ShouldUseWorkAttributes_WhenTranslationInfoLangMissing()
    {
        var workInfo = new WorkInfoDto
        {
            SourceId = "RJ1301",
            Title = "繁中版",
            WorkAttributes = "RG01020616,CHI_HANT,DLP",
        };

        var result = WorkLanguageSelectionPolicy.ResolveCurrentWorkLanguage(workInfo);

        Assert.Equal("繁体中文", result);
    }
}