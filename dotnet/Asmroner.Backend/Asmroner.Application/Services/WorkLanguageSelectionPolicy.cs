using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Utils;

namespace Asmroner.Application.Services;

public static class WorkLanguageSelectionPolicy
{
    public static readonly IReadOnlyList<string> DefaultPreferredLanguages = LanguagePriorityOptions.DefaultOrder;

    public static WorkLanguageSelectionResult SelectPreferredEdition(WorkInfoDto workInfo)
    {
        return SelectPreferredEdition(workInfo, DefaultPreferredLanguages);
    }

    public static WorkLanguageSelectionResult SelectPreferredEdition(WorkInfoDto workInfo, IReadOnlyList<string>? preferredLanguages)
    {
        ArgumentNullException.ThrowIfNull(workInfo);

        var effectivePreferredLanguages = BuildEffectivePreferredLanguages(preferredLanguages);
        var candidates = BuildCandidates(workInfo);
        foreach (var preferredLanguage in effectivePreferredLanguages)
        {
            var preferredCandidate = candidates.FirstOrDefault(candidate => string.Equals(candidate.Language, preferredLanguage, StringComparison.Ordinal));
            if (preferredCandidate is not null)
            {
                return new WorkLanguageSelectionResult
                {
                    SelectedWorkId = preferredCandidate.WorkId,
                    SelectedSourceId = preferredCandidate.SourceId,
                    SelectedTitle = preferredCandidate.Title,
                    SelectedLanguage = preferredCandidate.Language,
                    IsCurrentWork = preferredCandidate.IsCurrentWork,
                };
            }
        }

        var currentCandidate = candidates.FirstOrDefault(candidate => candidate.IsCurrentWork)
            ?? new WorkLanguageEditionCandidate
            {
                SourceId = SourceIdNormalizer.Normalize(workInfo.SourceId),
                Title = workInfo.Title,
                Language = ResolveCurrentWorkLanguage(workInfo),
                IsCurrentWork = true,
            };

        return new WorkLanguageSelectionResult
        {
            SelectedWorkId = currentCandidate.WorkId,
            SelectedSourceId = currentCandidate.SourceId,
            SelectedTitle = currentCandidate.Title,
            SelectedLanguage = currentCandidate.Language,
            IsCurrentWork = currentCandidate.IsCurrentWork,
        };
    }

    public static string ResolveCurrentWorkLanguage(WorkInfoDto workInfo)
    {
        ArgumentNullException.ThrowIfNull(workInfo);

        var translationInfoLanguage = LanguagePriorityOptions.NormalizeLanguage(workInfo.TranslationInfo.Lang);
        if (!string.IsNullOrWhiteSpace(translationInfoLanguage))
        {
            return translationInfoLanguage;
        }

        var workAttributeLanguage = ResolveLanguageFromWorkAttributes(workInfo.WorkAttributes);
        if (!string.IsNullOrWhiteSpace(workAttributeLanguage))
        {
            return workAttributeLanguage;
        }

        if (workInfo.TranslationInfo.IsOriginal)
        {
            return "日本語";
        }

        return string.Empty;
    }

    public static string NormalizeLanguage(string? rawLanguage)
    {
        return LanguagePriorityOptions.NormalizeLanguage(rawLanguage);
    }

    private static IReadOnlyList<string> BuildEffectivePreferredLanguages(IReadOnlyList<string>? preferredLanguages)
    {
        if (preferredLanguages is null || preferredLanguages.Count == 0)
        {
            return DefaultPreferredLanguages;
        }

        var normalized = new List<string>();
        foreach (var preferredLanguage in preferredLanguages)
        {
            var language = LanguagePriorityOptions.NormalizeLanguage(preferredLanguage);
            if (string.IsNullOrWhiteSpace(language))
            {
                continue;
            }

            if (normalized.Contains(language, StringComparer.Ordinal))
            {
                continue;
            }

            normalized.Add(language);
        }

        return normalized.Count == 0 ? DefaultPreferredLanguages : normalized;
    }

    private static IReadOnlyList<WorkLanguageEditionCandidate> BuildCandidates(WorkInfoDto workInfo)
    {
        var candidates = new List<WorkLanguageEditionCandidate>();

        var currentSourceId = SourceIdNormalizer.Normalize(workInfo.SourceId);
        if (!string.IsNullOrWhiteSpace(currentSourceId))
        {
            candidates.Add(new WorkLanguageEditionCandidate
            {
                WorkId = workInfo.Id,
                SourceId = currentSourceId,
                Title = workInfo.Title,
                Language = ResolveCurrentWorkLanguage(workInfo),
                IsCurrentWork = true,
            });
        }

        foreach (var edition in workInfo.OtherLanguageEditionsInDb)
        {
            var sourceId = SourceIdNormalizer.Normalize(edition.SourceId);
            var language = NormalizeLanguage(edition.Lang);
            if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(language))
            {
                continue;
            }

            if (candidates.Any(candidate => string.Equals(candidate.SourceId, sourceId, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            candidates.Add(new WorkLanguageEditionCandidate
            {
                WorkId = edition.Id,
                SourceId = sourceId,
                Title = edition.Title,
                Language = language,
                IsCurrentWork = false,
            });
        }

        return candidates;
    }

    private static string ResolveLanguageFromWorkAttributes(string workAttributes)
    {
        if (string.IsNullOrWhiteSpace(workAttributes))
        {
            return string.Empty;
        }

        foreach (var token in workAttributes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var normalized = NormalizeLanguage(token);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                return normalized;
            }
        }

        return string.Empty;
    }
}

public sealed class WorkLanguageSelectionResult
{
    public int SelectedWorkId { get; init; }

    public string SelectedSourceId { get; init; } = string.Empty;

    public string SelectedTitle { get; init; } = string.Empty;

    public string SelectedLanguage { get; init; } = string.Empty;

    public bool IsCurrentWork { get; init; }
}

public sealed class WorkLanguageEditionCandidate
{
    public int WorkId { get; init; }

    public string SourceId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Language { get; init; } = string.Empty;

    public bool IsCurrentWork { get; init; }
}