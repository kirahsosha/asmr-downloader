namespace Asmroner.Core.Configuration;

public sealed class SearchUiState
{
    public bool IncludeTranslationWorks { get; set; } = true;

    public bool QueueTranslationWorks { get; set; } = true;

    public string Tag { get; set; } = string.Empty;

    public bool TagExclude { get; set; }

    public string Circle { get; set; } = string.Empty;

    public bool CircleExclude { get; set; }

    public string Va { get; set; } = string.Empty;

    public bool VaExclude { get; set; }

    public string Duration { get; set; } = string.Empty;

    public bool DurationExclude { get; set; }

    public string Rate { get; set; } = string.Empty;

    public bool RateExclude { get; set; }

    public string Price { get; set; } = string.Empty;

    public bool PriceExclude { get; set; }

    public string Sell { get; set; } = string.Empty;

    public bool SellExclude { get; set; }

    public string Age { get; set; } = string.Empty;

    public bool AgeExclude { get; set; }

    public string Lang { get; set; } = string.Empty;

    public bool LangExclude { get; set; }
}
