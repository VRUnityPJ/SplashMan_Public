namespace Runtime.Bonus.Core;

/// <summary>
/// ボーナスのランク(獲得の難しさ)。
/// 表示側はこのランクに応じて文字色などの演出を変える
/// </summary>
public enum BonusRank
{
    /// <summary>
    /// 通常のボーナス
    /// </summary>
    Normal,

    /// <summary>
    /// 獲得が難しいボーナス
    /// </summary>
    Rare,
}
