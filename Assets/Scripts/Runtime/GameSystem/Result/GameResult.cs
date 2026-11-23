using System.Collections.Generic;
using System.Linq;
using Runtime.Bonus;
using Runtime.Bonus.Core;
using Runtime.Scores;

namespace Runtime.GameSystem.Result;

/// <summary>
/// ゲームの結果を表す構造体
/// </summary>
public readonly struct GameResult
{
    /// <summary>
    /// ボーナス加算前の基本スコア
    /// </summary>
    public readonly Score BaseScore;

    /// <summary>
    /// ボーナス加算後の最終スコア
    /// </summary>
    public readonly Score FinalScore;

    /// <summary>
    /// 獲得したボーナスを種類ごとにまとめたリスト(初めて獲得した順)
    /// </summary>
    public readonly IReadOnlyList<BonusSummary> Bonuses;

    /// <summary>
    /// 集計済みのボーナスからゲーム結果を生成する
    /// 通常はCreateを使う。デバッグなどでダミーの結果を作りたいときに使う
    /// </summary>
    /// <param name="baseScore"></param>
    /// <param name="bonuses"></param>
    public GameResult(Score baseScore, IReadOnlyList<BonusSummary> bonuses)
    {
        BaseScore = baseScore;
        Bonuses = bonuses;
        FinalScore = bonuses.Aggregate(baseScore, (sum, b) => sum + b.TotalAmount);
    }

    /// <summary>
    /// 基本スコアと獲得したボーナスの一覧からゲーム結果を確定する
    /// 同じボーナスは1つにまとめ、獲得回数とスコアの合計を集計する
    /// </summary>
    /// <param name="baseScore"></param>
    /// <param name="earnedBonuses"></param>
    public static GameResult Create(Score baseScore, IEnumerable<BonusCore> earnedBonuses)
    {
        // ボーナスは種類ごとにSingletonで登録されているので、インスタンス単位でまとめる
        // GroupByは最初に出現した順を保つので、初めて獲得した順に並ぶ
        // ToArrayで確定時点のスナップショットにする
        var summaries = earnedBonuses
            .GroupBy(bonus => bonus)
            .Select(group => new BonusSummary(
                group.Key.Name,
                group.Count(),
                group.Aggregate(Score.Zero, (sum, bonus) => sum + bonus.Amount),
                group.Key.Rank))
            .ToArray();

        return new GameResult(baseScore, summaries);
    }
}
