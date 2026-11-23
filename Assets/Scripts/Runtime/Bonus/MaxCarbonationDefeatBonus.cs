using Runtime.Bonus.Core;
using Runtime.Enemy;
using Runtime.Scores;

namespace Runtime.Bonus;

/// <summary>
/// 炭酸ゲージをMaxにした状態で敵を倒した時に得られるボーナス
/// </summary>
public class MaxCarbonationDefeatBonus : BonusCore
{
    private readonly AttackEnemy _attackEnemy;

    public MaxCarbonationDefeatBonus(PlayerScore playerScore, AttackEnemy attackEnemy) : base(playerScore)
    {
        _attackEnemy = attackEnemy;
    }

    /// <inheritdoc />
    public override string Name => "炭酸MAX撃破ボーナス";

    /// <inheritdoc />
    public override Score Amount { get; } = new(500);

    /// <inheritdoc />
    protected override BonusRequired Register() =>
        BonusTrigger
            .Watch(_attackEnemy.OnAttacked) // ここで対象のストリームを指定する
            .When(info => info.CarbonationValue == info.MaxCarbonationValue);　// ボーナスを獲得する条件をかく. この条件は適当
}
