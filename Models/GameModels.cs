namespace sperger_cockroach.Models;

/// <summary>
/// 定義遊戲流程目前所在階段。
/// </summary>
public enum GamePhase
{
    // 顯示故事前言。
    Intro,
    // 執行即時伏擊。
    Playing,
    // 顯示章節過場。
    ChapterComplete,
    // 顯示結算日誌。
    Result
}

/// <summary>
/// 定義洞穴中可能出現的標靶。
/// </summary>
public enum TargetKind
{
    // 普通異變蟲。
    CommonBug,
    // 稀有晶岩蟲。
    RareBug,
    // 不應攻擊的友善生物。
    FriendlyCreature,
    // 毒孢子或高硬礦石。
    Hazard,
    // 啟動超感官模式的氣泡。
    ActiveBubble
}

/// <summary>
/// 代表畫面上單一可點擊標靶。
/// </summary>
public sealed class GameTarget
{
    // 唯一識別碼避免重複處理同一標靶。
    public Guid Id { get; init; } = Guid.NewGuid();
    // 洞穴索引決定顯示位置。
    public int HoleIndex { get; init; }
    // 標靶類型決定遊戲效果。
    public TargetKind Kind { get; init; }
}

/// <summary>
/// 對應 Agent 規格中的關卡故事回應。
/// </summary>
public sealed class StageStoryResponse
{
    // 章節標題。
    public string ChapterTitle { get; set; } = string.Empty;
    // 章節故事。
    public string StoryText { get; set; } = string.Empty;
    // 斯柏格獨白。
    public string SpergerMonologue { get; set; } = string.Empty;
}

/// <summary>
/// 對應 Agent 規格中的遊戲結算評價。
/// </summary>
public sealed class EvaluationResponse
{
    // SSS、A、B、C 或 D 評級。
    public string Rank { get; set; } = string.Empty;
    // 生態日誌標題。
    public string LogTitle { get; set; } = string.Empty;
    // 狩獵摘要。
    public string SummaryText { get; set; } = string.Empty;
    // 斯柏格第一人稱評語。
    public string SpergerQuote { get; set; } = string.Empty;
}

/// <summary>
/// 對應 Agent 規格中的單局遊戲統計。
/// </summary>
public sealed class GameSessionResult
{
    // 最終累積分數。
    public int FinalScore { get; set; }
    // 最大連擊數。
    public int MaxCombo { get; set; }
    // 成功捕食異變蟲數。
    public int BugsCaught { get; set; }
    // 誤傷友軍次數。
    public int FriendlyMiscegenationCount { get; set; }
    // 擊中危險物次數。
    public int BombsHitCount { get; set; }
}

/// <summary>
/// 對應 config.yaml 的完整遊戲設定。
/// </summary>
public sealed class GameConfig
{
    // 網站標題與品牌內容。
    public SiteConfig Site { get; set; } = new();
    // 開場故事內容。
    public IntroConfig Intro { get; set; } = new();
    // 版面與時間參數。
    public LayoutConfig Layout { get; set; } = new();
    // 得分與特殊模式參數。
    public GameplayConfig Gameplay { get; set; } = new();
    // 三章劇情資料。
    public List<ChapterConfig> Chapters { get; set; } = [];
    // 標靶圖鑑資料。
    public List<TargetConfig> Targets { get; set; } = [];
    // 結算評語範本。
    public List<EvaluationConfig> Evaluations { get; set; } = [];
}

/// <summary>
/// 定義網站品牌文字。
/// </summary>
public sealed class SiteConfig
{
    // 瀏覽器標題。
    public string Title { get; set; } = "Sperger Cockroach";
    // 品牌名稱。
    public string Brand { get; set; } = "蟲鳴谷";
    // 品牌副標。
    public string Subtitle { get; set; } = "異世界狩獵紀錄";
    // 頁尾觀測站名稱。
    public string FooterLabel { get; set; } = "孕蟲河生態獵場";
}

/// <summary>
/// 定義開場故事文字。
/// </summary>
public sealed class IntroConfig
{
    // 開場眉題。
    public string Kicker { get; set; } = string.Empty;
    // 主標題第一行。
    public string Headline { get; set; } = string.Empty;
    // 主標題強調文字。
    public string HeadlineAccent { get; set; } = string.Empty;
    // 世界觀摘要。
    public string Lead { get; set; } = string.Empty;
    // 主角引言。
    public string Quote { get; set; } = string.Empty;
    // 引言署名。
    public string QuoteBy { get; set; } = string.Empty;
    // 開始按鈕文字。
    public string StartButton { get; set; } = "進入伏擊位置";
}

/// <summary>
/// 定義可調整的版面與時間參數。
/// </summary>
public sealed class LayoutConfig
{
    // 每章秒數。
    public int ChapterDurationSeconds { get; set; } = 20;
    // 獵場洞穴數量。
    public int HoleCount { get; set; } = 9;
    // 獵場欄數。
    public int HoleColumns { get; set; } = 3;
    // 初始體力。
    public int StartingHealth { get; set; } = 3;
}

/// <summary>
/// 定義遊戲分數與特殊模式參數。
/// </summary>
public sealed class GameplayConfig
{
    // 普通蟲得分。
    public int CommonBugScore { get; set; } = 100;
    // 稀有蟲得分。
    public int RareBugScore { get; set; } = 300;
    // 友善生物扣分。
    public int FriendlyPenalty { get; set; } = 150;
    // 危險物扣分。
    public int HazardPenalty { get; set; } = 300;
    // 啟動狂暴所需連擊。
    public int FrenzyCombo { get; set; } = 10;
    // 狂暴持續秒數。
    public int FrenzyDurationSeconds { get; set; } = 6;
    // 狂暴得分倍數。
    public int FrenzyMultiplier { get; set; } = 2;
    // 超感官持續秒數。
    public int BulletTimeDurationSeconds { get; set; } = 3;
}

/// <summary>
/// 定義單一章節內容。
/// </summary>
public sealed class ChapterConfig
{
    // 章節標題。
    public string Title { get; set; } = string.Empty;
    // 英文章節標記。
    public string Kicker { get; set; } = string.Empty;
    // 過場故事。
    public string Story { get; set; } = string.Empty;
    // 通關對話。
    public string Quote { get; set; } = string.Empty;
}

/// <summary>
/// 定義圖鑑中的單一標靶。
/// </summary>
public sealed class TargetConfig
{
    // 對應 TargetKind 的鍵。
    public string Kind { get; set; } = string.Empty;
    // 圖鑑分類。
    public string Category { get; set; } = string.Empty;
    // 顯示名稱。
    public string Name { get; set; } = string.Empty;
    // 分數標籤。
    public string ScoreLabel { get; set; } = string.Empty;
    // 說明文字。
    public string Description { get; set; } = string.Empty;
    // 標靶中文字形。
    public string Glyph { get; set; } = string.Empty;
    // 無障礙標籤。
    public string AccessibleLabel { get; set; } = string.Empty;
    // 隨機出現權重。
    public int Weight { get; set; } = 1;
}

/// <summary>
/// 定義單一結算評語範本。
/// </summary>
public sealed class EvaluationConfig
{
    // 評級名稱。
    public string Rank { get; set; } = "B";
    // 達成此評級的最低調整分數。
    public int MinimumScore { get; set; }
    // 日誌標題。
    public string LogTitle { get; set; } = string.Empty;
    // 摘要範本，可使用統計變數。
    public string SummaryTemplate { get; set; } = string.Empty;
    // 斯柏格評語。
    public string Quote { get; set; } = string.Empty;
}
