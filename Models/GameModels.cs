namespace sperger_cockroach.Models;

public enum GamePhase
{
    Ready,
    Playing,
    ChapterComplete,
    Result
}

public enum InsectCategory
{
    NormalBug,
    FatBug,
    BlueBerry,
    LadyBug,
    Mouse,
    PileOfWood
}

public enum DifficultyLevel
{
    Hell,
    Hard,
    Normal,
    Easy,
    Beginner
}

/// <summary>
/// 對應 config.yaml 的完整遊戲設定。
/// </summary>
public sealed class GameConfig
{
    public SiteConfig Site { get; set; } = new();
    public IntroConfig Intro { get; set; } = new();
    public LayoutConfig Layout { get; set; } = new();
    public GameplayConfig Gameplay { get; set; } = new();
    public List<DifficultyConfig> Difficulties { get; set; } = [];
    public List<ChapterConfig> Chapters { get; set; } = [];
    public List<TargetConfig> Targets { get; set; } = [];
    public List<EvaluationConfig> Evaluations { get; set; } = [];
    public List<string> SpergerPhrases { get; set; } = [];
    public LoreConfig Lore { get; set; } = new();
    public TreasureConfig Treasure { get; set; } = new();
}

public sealed class SiteConfig
{
    public string Title { get; set; } = "Sperger Cockroach";
    public string Brand { get; set; } = "蟲鳴谷";
    public string Subtitle { get; set; } = "異世界狩獵紀錄";
    public string FooterLabel { get; set; } = "孕蟲河生態獵場";
}

public sealed class IntroConfig
{
    public string Kicker { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string HeadlineAccent { get; set; } = string.Empty;
    public string Lead { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string QuoteBy { get; set; } = string.Empty;
    public string StartButton { get; set; } = "前往獵場";
}

public sealed class LayoutConfig
{
    public int BoardRows { get; set; } = 10;
    public int BoardColumns { get; set; } = 10;
    public int ChapterDurationSeconds { get; set; } = 20;
    public int HitFeedbackMilliseconds { get; set; } = 300;
}

public sealed class GameplayConfig
{
    public int TargetScore { get; set; } = 100;
    public int MaximumMultiplier { get; set; } = 4;
    public int WoodTimePenaltySeconds { get; set; } = 5;
}

public sealed class DifficultyConfig
{
    public DifficultyLevel Level { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SpawnIntervalMilliseconds { get; set; }
}

public sealed class ChapterConfig
{
    public string Title { get; set; } = string.Empty;
    public string Kicker { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
}

public sealed class TargetConfig
{
    public InsectCategory Category { get; set; }
    public string Group { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ScoreLabel { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AccessibleLabel { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Reaction { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Weight { get; set; }
}

public sealed class EvaluationConfig
{
    public string Rank { get; set; } = string.Empty;
    public int MinimumScore { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Review { get; set; } = string.Empty;
}

public sealed class LoreConfig
{
    public string Heading { get; set; } = "斯柏格．費里尼";
    public string Lead { get; set; } = string.Empty;
    public List<LoreSectionConfig> Sections { get; set; } = [];
}

public sealed class LoreSectionConfig
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public sealed class TreasureConfig
{
    public string Title { get; set; } = "斯柏格撿金幣";
    public string Introduction { get; set; } = string.Empty;
    public int Rows { get; set; } = 19;
    public int Columns { get; set; } = 9;
    public int CoinCount { get; set; } = 5;
    public int ScorePerCoin { get; set; } = 20;
    public List<string> Phrases { get; set; } = [];
}

/// <summary>
/// 保存一局跨章遠征的可變狀態。
/// </summary>
public sealed class GameSessionState
{
    public int Score { get; set; }
    public int Multiplier { get; set; } = 1;
    public int ChapterNumber { get; set; } = 1;
    public int TimePenaltySeconds { get; set; }
    public int SuccessfulHits { get; set; }
    public int Misses { get; set; }
}

public sealed record TargetEffectResult(string Message, bool IsBeneficial);

public sealed record ActiveTarget(int CellIndex, TargetConfig Definition);

public sealed record TreasureRevealResult(bool WasRevealed, bool IsCoin, int AwardedScore);

public sealed class EvaluationResponse
{
    public string Rank { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Review { get; set; } = string.Empty;
}

/// <summary>
/// 寫入瀏覽器本機儲存空間的狩獵成績。
/// </summary>
public sealed class ScoreRecord
{
    public string Difficulty { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Rank { get; set; } = string.Empty;
    public int ChapterReached { get; set; }
    public int Multiplier { get; set; }
    public int SuccessfulHits { get; set; }
    public int Misses { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset EndedAt { get; set; }
    public double ActiveSeconds { get; set; }
}
