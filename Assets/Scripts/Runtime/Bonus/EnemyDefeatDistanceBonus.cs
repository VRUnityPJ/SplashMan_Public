using Runtime.Bonus.Core;
using Runtime.Enemy;
using Runtime.Scores;

namespace Runtime.Bonus;

public class EnemyDefeatDistanceBonus : BonusCore
{
    private readonly AttackEnemy _attackEnemy;

    public EnemyDefeatDistanceBonus(PlayerScore playerScore, AttackEnemy attackEnemy) : base(playerScore)
    {
        _attackEnemy = attackEnemy;
    }

    public override string Name => "敵撃破距離ボーナス";
    public override Score Amount { get; } = new(500);
    protected override BonusRequired Register() =>
        BonusTrigger
            .Watch(_attackEnemy.OnDefeatDistance)
            .When(info => info > 10f); // 敵撃破時の距離が10以上の場合にボーナスを獲得する条件

}