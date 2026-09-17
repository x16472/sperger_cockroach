using sperger_cockroach.Models;

namespace sperger_cockroach.Services;

/// <summary>
/// 集中處理與畫面無關的狩獵及尋寶規則。
/// </summary>
public sealed class GameRulesService
{
    public TargetConfig SelectTarget(IReadOnlyList<TargetConfig> targets, int roll)
    {
        var total_weight = targets.Sum(target => target.Weight);
        if (total_weight <= 0 || roll < 0 || roll >= total_weight)
        {
            throw new ArgumentOutOfRangeException(nameof(roll), "亂數必須位於標靶總權重範圍內。");
        }

        foreach (var target in targets)
        {
            if (roll < target.Weight)
            {
                return target;
            }

            roll -= target.Weight;
        }

        throw new InvalidOperationException("無法依權重選出標靶。");
    }

    public TargetEffectResult ApplyTarget(GameSessionState session, TargetConfig target, GameplayConfig gameplay)
    {
        switch (target.Category)
        {
            case InsectCategory.NormalBug:
            case InsectCategory.FatBug:
            case InsectCategory.LadyBug:
                var gained_score = target.Score * session.Multiplier;
                session.Score += gained_score;
                session.SuccessfulHits++;
                return new TargetEffectResult($"+{gained_score} 分（×{session.Multiplier}）", true);
            case InsectCategory.BlueBerry:
                session.Multiplier = Math.Min(session.Multiplier + 1, gameplay.MaximumMultiplier);
                session.SuccessfulHits++;
                return new TargetEffectResult($"積分倍率提升為 ×{session.Multiplier}", true);
            case InsectCategory.Mouse:
                session.Score += target.Score;
                session.Multiplier = 1;
                session.SuccessfulHits++;
                return new TargetEffectResult($"{target.Score} 分，積分倍率已取消", false);
            case InsectCategory.PileOfWood:
                session.TimePenaltySeconds += gameplay.WoodTimePenaltySeconds;
                session.SuccessfulHits++;
                return new TargetEffectResult($"本章時間 -{gameplay.WoodTimePenaltySeconds} 秒", false);
            default:
                throw new ArgumentOutOfRangeException(nameof(target), "不支援的標靶種類。");
        }
    }

    public bool ShouldFinishExpedition(int score, int chapter_number, GameConfig config)
    {
        return score >= config.Gameplay.TargetScore || chapter_number >= config.Chapters.Count;
    }

    public EvaluationResponse Evaluate(int score, IReadOnlyList<EvaluationConfig> evaluations)
    {
        var evaluation = evaluations
            .OrderByDescending(item => item.MinimumScore)
            .FirstOrDefault(item => score >= item.MinimumScore)
            ?? throw new InvalidOperationException("找不到符合分數的結算評語。");

        return new EvaluationResponse
        {
            Rank = evaluation.Rank,
            Title = evaluation.Title,
            Review = evaluation.Review
        };
    }

    public HashSet<int> PlaceTreasure(int rows, int columns, int coin_count, Random random)
    {
        var cell_count = checked(rows * columns);
        if (rows <= 0 || columns <= 0 || coin_count <= 0 || coin_count > cell_count)
        {
            throw new ArgumentOutOfRangeException(nameof(coin_count), "金幣數必須落在盤面容量內。");
        }

        var coin_cells = new HashSet<int>();
        while (coin_cells.Count < coin_count)
        {
            coin_cells.Add(random.Next(cell_count));
        }

        return coin_cells;
    }

    public TreasureRevealResult RevealTreasure(
        HashSet<int> revealed_cells,
        IReadOnlySet<int> coin_cells,
        int cell_index,
        int score_per_coin)
    {
        if (!revealed_cells.Add(cell_index))
        {
            return new TreasureRevealResult(false, coin_cells.Contains(cell_index), 0);
        }

        var is_coin = coin_cells.Contains(cell_index);
        return new TreasureRevealResult(true, is_coin, is_coin ? score_per_coin : 0);
    }
}
