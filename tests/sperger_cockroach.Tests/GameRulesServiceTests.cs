using sperger_cockroach.Models;
using sperger_cockroach.Services;
using Xunit;

namespace sperger_cockroach.Tests;

public sealed class GameRulesServiceTests
{
    private readonly GameRulesService _rules = new();

    [Theory]
    [InlineData(0, InsectCategory.NormalBug)]
    [InlineData(37, InsectCategory.NormalBug)]
    [InlineData(38, InsectCategory.FatBug)]
    [InlineData(47, InsectCategory.FatBug)]
    [InlineData(48, InsectCategory.BlueBerry)]
    [InlineData(62, InsectCategory.BlueBerry)]
    [InlineData(63, InsectCategory.LadyBug)]
    [InlineData(82, InsectCategory.LadyBug)]
    [InlineData(83, InsectCategory.Mouse)]
    [InlineData(92, InsectCategory.Mouse)]
    [InlineData(93, InsectCategory.PileOfWood)]
    [InlineData(99, InsectCategory.PileOfWood)]
    public void SelectTarget_UsesExpectedWeightBoundaries(int roll, InsectCategory expected)
    {
        var selected = _rules.SelectTarget(CreateTargets(), roll);

        Assert.Equal(expected, selected.Category);
    }

    [Fact]
    public void ApplyTarget_BlueBerryCapsMultiplierAndMouseResetsIt()
    {
        var session = new GameSessionState { Multiplier = 4, Score = 30 };
        var gameplay = new GameplayConfig { MaximumMultiplier = 4, WoodTimePenaltySeconds = 5 };

        _rules.ApplyTarget(session, Target(InsectCategory.BlueBerry, 0, 15), gameplay);
        Assert.Equal(4, session.Multiplier);

        _rules.ApplyTarget(session, Target(InsectCategory.Mouse, -15, 10), gameplay);
        Assert.Equal(1, session.Multiplier);
        Assert.Equal(15, session.Score);
    }

    [Fact]
    public void ApplyTarget_WoodAddsFiveSecondPenalty()
    {
        var session = new GameSessionState();
        var gameplay = new GameplayConfig { MaximumMultiplier = 4, WoodTimePenaltySeconds = 5 };

        _rules.ApplyTarget(session, Target(InsectCategory.PileOfWood, 0, 7), gameplay);

        Assert.Equal(5, session.TimePenaltySeconds);
    }

    [Theory]
    [InlineData(99, 1, false)]
    [InlineData(100, 1, true)]
    [InlineData(49, 3, true)]
    public void ShouldFinishExpedition_UsesScoreOrFinalChapter(int score, int chapter, bool expected)
    {
        Assert.Equal(expected, _rules.ShouldFinishExpedition(score, chapter, CreateConfig()));
    }

    [Theory]
    [InlineData(49, "C～D")]
    [InlineData(50, "A～B")]
    [InlineData(99, "A～B")]
    [InlineData(100, "SSS")]
    public void Evaluate_UsesDocumentedBoundaries(int score, string expected)
    {
        var result = _rules.Evaluate(score, CreateEvaluations());

        Assert.Equal(expected, result.Rank);
    }

    [Fact]
    public void TreasurePlacementCreatesUniqueCellsAndRepeatRevealDoesNotScore()
    {
        var coins = _rules.PlaceTreasure(19, 9, 5, new Random(42));
        var revealed = new HashSet<int>();
        var coin = coins.First();

        var first = _rules.RevealTreasure(revealed, coins, coin, 20);
        var repeated = _rules.RevealTreasure(revealed, coins, coin, 20);

        Assert.Equal(5, coins.Count);
        Assert.All(coins, cell => Assert.InRange(cell, 0, 170));
        Assert.True(first.IsCoin);
        Assert.Equal(20, first.AwardedScore);
        Assert.False(repeated.WasRevealed);
        Assert.Equal(0, repeated.AwardedScore);
    }

    [Fact]
    public void ValidateRejectsMissingTargetCategory()
    {
        var config = CreateConfig();
        config.Targets.RemoveAt(config.Targets.Count - 1);

        var exception = Assert.Throws<InvalidOperationException>(() => GameConfigService.Validate(config));

        Assert.Contains("六種", exception.Message);
    }

    private static GameConfig CreateConfig()
    {
        return new GameConfig
        {
            Layout = new LayoutConfig(),
            Gameplay = new GameplayConfig(),
            Chapters = [new(), new(), new()],
            Difficulties =
            [
                new() { Level = DifficultyLevel.Hell, SpawnIntervalMilliseconds = 350 },
                new() { Level = DifficultyLevel.Hard, SpawnIntervalMilliseconds = 500 },
                new() { Level = DifficultyLevel.Normal, SpawnIntervalMilliseconds = 750 },
                new() { Level = DifficultyLevel.Easy, SpawnIntervalMilliseconds = 1000 },
                new() { Level = DifficultyLevel.Beginner, SpawnIntervalMilliseconds = 1300 }
            ],
            Targets = CreateTargets(),
            Evaluations = CreateEvaluations(),
            SpergerPhrases = ["測試語錄"],
            Treasure = new TreasureConfig { Phrases = ["大吉大利"] }
        };
    }

    private static List<TargetConfig> CreateTargets()
    {
        return
        [
            Target(InsectCategory.NormalBug, 10, 38),
            Target(InsectCategory.FatBug, 20, 10),
            Target(InsectCategory.BlueBerry, 0, 15),
            Target(InsectCategory.LadyBug, 5, 20),
            Target(InsectCategory.Mouse, -15, 10),
            Target(InsectCategory.PileOfWood, 0, 7)
        ];
    }

    private static List<EvaluationConfig> CreateEvaluations()
    {
        return
        [
            new() { Rank = "SSS", MinimumScore = 100 },
            new() { Rank = "A～B", MinimumScore = 50 },
            new() { Rank = "C～D", MinimumScore = -999999 }
        ];
    }

    private static TargetConfig Target(InsectCategory category, int score, int weight)
    {
        return new TargetConfig { Category = category, Score = score, Weight = weight };
    }
}
