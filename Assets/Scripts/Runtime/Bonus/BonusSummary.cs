using Runtime.Bonus.Core;
using Runtime.Scores;

namespace Runtime.Bonus;

/// <summary>
/// 同じボーナスをまとめた集計結果
/// </summary>
public readonly struct BonusSummary
{
    /// <summary>
    /// ボーナスの表示名
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 獲得回数
    /// </summary>
    public readonly int Count;

    /// <summary>
    /// このボーナスで獲得したスコアの合計
    /// </summary>
    public readonly Score TotalAmount;

    /// <summary>
    /// ボーナスのランク
    /// </summary>
    public readonly BonusRank Rank;

    public BonusSummary(string name, int count, Score totalAmount, BonusRank rank = BonusRank.Normal)
    {
        Name = name;
        Count = count;
        TotalAmount = totalAmount;
        Rank = rank;
    }
}
