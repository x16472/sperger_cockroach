using sperger_cockroach.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace sperger_cockroach.Services;

/// <summary>
/// 從網站靜態資產讀取、解析並驗證遊戲設定。
/// </summary>
public sealed class GameConfigService
{
    private readonly HttpClient _http_client;
    private readonly IDeserializer _yaml_deserializer;

    public GameConfigService(HttpClient http_client)
    {
        _http_client = http_client;
        _yaml_deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public async Task<GameConfig> LoadAsync(CancellationToken cancellation_token = default)
    {
        var yaml_text = await _http_client.GetStringAsync("config.yaml", cancellation_token);
        var game_config = _yaml_deserializer.Deserialize<GameConfig>(yaml_text)
            ?? throw new InvalidOperationException("config.yaml 未包含有效的遊戲設定。");

        Validate(game_config);
        return game_config;
    }

    public static void Validate(GameConfig game_config)
    {
        if (game_config.Layout.BoardRows != 10 || game_config.Layout.BoardColumns != 10)
        {
            throw new InvalidOperationException("主遊戲盤面必須設定為 10 × 10。");
        }

        if (game_config.Layout.ChapterDurationSeconds <= 0 || game_config.Layout.HitFeedbackMilliseconds <= 0)
        {
            throw new InvalidOperationException("章節秒數與命中回饋時間必須大於零。");
        }

        if (game_config.Chapters.Count != 3)
        {
            throw new InvalidOperationException("config.yaml 必須包含三個 chapters 項目。");
        }

        if (game_config.Difficulties.Count != Enum.GetValues<DifficultyLevel>().Length ||
            game_config.Difficulties.Any(difficulty => difficulty.SpawnIntervalMilliseconds <= 0) ||
            game_config.Difficulties.Select(difficulty => difficulty.Level).Distinct().Count() != game_config.Difficulties.Count)
        {
            throw new InvalidOperationException("config.yaml 必須包含五種不重複且速度有效的難度。");
        }

        var expected_categories = Enum.GetValues<InsectCategory>();
        if (game_config.Targets.Count != expected_categories.Length ||
            game_config.Targets.Select(target => target.Category).Distinct().Count() != expected_categories.Length ||
            expected_categories.Any(category => game_config.Targets.All(target => target.Category != category)))
        {
            throw new InvalidOperationException("config.yaml 必須各自定義六種不重複的標靶。");
        }

        if (game_config.Targets.Any(target => target.Weight <= 0) || game_config.Targets.Sum(target => target.Weight) != 100)
        {
            throw new InvalidOperationException("六種標靶權重必須皆大於零且總和為 100。");
        }

        if (game_config.Gameplay.TargetScore <= 0 || game_config.Gameplay.MaximumMultiplier < 1 ||
            game_config.Gameplay.WoodTimePenaltySeconds <= 0)
        {
            throw new InvalidOperationException("得分目標、倍率上限與木柴扣時設定必須有效。");
        }

        if (game_config.Evaluations.Count == 0 || game_config.SpergerPhrases.Count == 0)
        {
            throw new InvalidOperationException("至少需要一筆結算評語與斯柏格語錄。");
        }

        var treasure = game_config.Treasure;
        if (treasure.Rows <= 0 || treasure.Columns <= 0 || treasure.CoinCount <= 0 ||
            treasure.CoinCount > treasure.Rows * treasure.Columns || treasure.ScorePerCoin <= 0 || treasure.Phrases.Count == 0)
        {
            throw new InvalidOperationException("尋寶遊戲的盤面、金幣與吉祥話設定無效。");
        }
    }
}
