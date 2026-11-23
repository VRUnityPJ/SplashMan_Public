namespace Runtime.Carbonation;

/// <summary>
/// 攻撃結果を表す構造体
/// </summary>
public readonly struct AttackResult
{
    /// <summary>
    /// 攻撃後の炭酸量
    /// </summary>
    public readonly Carbonation CarbonationValue;

    /// <summary>
    /// ペットボトルの最大炭酸量
    /// </summary>
    public readonly Carbonation MaxCarbonationValue;

    /// <summary>
    /// 一回の攻撃で倒した敵の数
    /// </summary>
    public readonly int DefeatedCount;

    /// <summary>
    /// 一回の攻撃で与えた敵の数
    /// </summary>
    public readonly int AttackCount;

    public AttackResult(Carbonation carbonation, Carbonation maxCarbonationValue, int defeatedCount, int attackCount)
    {
        CarbonationValue = carbonation;
        MaxCarbonationValue = maxCarbonationValue;
        DefeatedCount = defeatedCount;
        AttackCount = attackCount;
    }
}