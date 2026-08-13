// 匯入遊戲設定模型。
using sperger_cockroach.Models;
// 匯入 YAML 反序列化功能。
using YamlDotNet.Serialization;
// 匯入 camelCase 命名規則。
using YamlDotNet.Serialization.NamingConventions;

namespace sperger_cockroach.Services;

/// <summary>
/// 從網站靜態資產讀取並解析 config.yaml。
/// </summary>
public sealed class GameConfigService
{
    // 儲存目前網站基底網址的 HTTP 用戶端。
    private readonly HttpClient _http_client;
    // 儲存支援 camelCase 的 YAML 解析器。
    private readonly IDeserializer _yaml_deserializer;

    /// <summary>
    /// 建立全域遊戲設定讀取服務。
    /// </summary>
    public GameConfigService(HttpClient http_client)
    {
        // 保存由依賴注入提供的 HTTP 用戶端。
        _http_client = http_client;
        // 建立忽略未知欄位的 YAML 解析器，便於日後擴充設定。
        _yaml_deserializer = new DeserializerBuilder()
            // 將 YAML camelCase 鍵名對應到 C# PascalCase 屬性。
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            // 忽略程式尚未使用的新設定欄位。
            .IgnoreUnmatchedProperties()
            // 完成解析器建立。
            .Build();
    }

    /// <summary>
    /// 非同步讀取並驗證 config.yaml。
    /// </summary>
    public async Task<GameConfig> LoadAsync(CancellationToken cancellation_token = default)
    {
        // 從網站基底路徑讀取納入版控的 YAML 文字。
        var yaml_text = await _http_client.GetStringAsync("config.yaml", cancellation_token);
        // 將 YAML 轉換為強型別設定。
        var game_config = _yaml_deserializer.Deserialize<GameConfig>(yaml_text)
            // 無法產生設定時回報明確錯誤。
            ?? throw new InvalidOperationException("config.yaml 未包含有效的遊戲設定。");
        // 驗證會影響陣列索引與計時的必要設定。
        Validate(game_config);
        // 回傳可供整個遊戲使用的設定。
        return game_config;
    }

    /// <summary>
    /// 驗證遊戲執行必要的設定範圍。
    /// </summary>
    private static void Validate(GameConfig game_config)
    {
        // 遊戲至少需要一個章節。
        if (game_config.Chapters.Count == 0)
        {
            // 提示內容維護者補上章節。
            throw new InvalidOperationException("config.yaml 至少需要一個 chapters 項目。");
        }

        // 洞穴數與欄數都必須大於零。
        if (game_config.Layout.HoleCount <= 0 || game_config.Layout.HoleColumns <= 0)
        {
            // 防止獵場產生無效的 CSS Grid 與隨機索引。
            throw new InvalidOperationException("config.yaml 的 holeCount 與 holeColumns 必須大於零。");
        }

        // 每章秒數與初始體力都必須大於零。
        if (game_config.Layout.ChapterDurationSeconds <= 0 || game_config.Layout.StartingHealth <= 0)
        {
            // 防止遊戲開始後立即結束。
            throw new InvalidOperationException("config.yaml 的章節秒數與初始體力必須大於零。");
        }
    }
}
