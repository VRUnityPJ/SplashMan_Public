using Runtime.Bonus.Core;
using Runtime.Enemy;
using Runtime.Scores;

namespace Runtime.Bonus;

/// <summary>
/// 炭酸ゲージが0の状態で敵を倒した時に得られるボーナス
/// </summary>
public class ZeroCarbonationDefeatBonus : BonusCore
{
    private readonly AttackEnemy _attackEnemy;

    public ZeroCarbonationDefeatBonus(PlayerScore playerScore, AttackEnemy attackEnemy) : base(playerScore)
    {
        _attackEnemy = attackEnemy;
    }

    /// <inheritdoc />
    public override string Name => "炭酸ゼロ撃破ボーナス";

    /// <inheritdoc />
    public override Score Amount { get; } = new(5000);

    /// <inheritdoc />
    public override BonusRank Rank => BonusRank.Rare;

    /// <inheritdoc />
    protected override BonusRequired Register() =>
        BonusTrigger
            .Watch(_attackEnemy.OnAttacked)
            // 炭酸ゲージが0のボトルで、敵を1体以上倒した場合にボーナスを獲得する条件
            .When(info => info.DefeatedCount > 0 && info.CarbonationValue.Value <= 0f);
}
