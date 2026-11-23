using Runtime.Bonus.Core;
using Runtime.Enemy;
using Runtime.Scores;

namespace Runtime.Bonus;

/// <summary>
/// 範囲内の敵数に応じてボーナスを付与するクラス
/// </summary>
public class EnemyNumInRangeBonus : BonusCore
{
    private readonly AttackEnemy _attackEnemy;

    public EnemyNumInRangeBonus(PlayerScore playerScore, AttackEnemy attackEnemy) : base(playerScore)
    {
        _attackEnemy = attackEnemy;
    }

    /// <inheritdoc />
    public override string Name => "同時ダメージ付与ボーナス";

    /// <inheritdoc />
    public override Score Amount { get; } = new(300);

    /// <inheritdoc />
    protected override BonusRequired Register() =>
        BonusTrigger
            .Watch(_attackEnemy.OnAttacked)
            .When(info => info.AttackCount >= 3); // 同時に3体以上の敵にダメージを与えた場合にボーナスを獲得する条件
}