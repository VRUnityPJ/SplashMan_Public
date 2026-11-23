using System;
using R3;

namespace Runtime.Enemy
{
    public class EnemyStatus : IDisposable
    {
        private readonly EnemyMasterData _enemyMasterData;

        public int MaxHp => _enemyMasterData.Hp;
        private readonly ReactiveProperty<int> _hp;
        public ReadOnlyReactiveProperty<int> Hp { get; }
        public int Attack => _enemyMasterData.AttackPower;
        public float Speed => _enemyMasterData.MoveSpeed;
        public int Defense => _enemyMasterData.DefensePower;

        public EnemyStatus(EnemyMasterData masterData)
        {
            _enemyMasterData = masterData;
            _hp = new ReactiveProperty<int>(_enemyMasterData.Hp);
            Hp = _hp.ToReadOnlyReactiveProperty();
        }

        public void TakeDamage(int damage)
        {
            // ダメージ計算: ダメージ - 防御力。最低でも1ダメージは与える
            var actualDamage = Math.Max(1, damage -  Defense);
            _hp.Value -= actualDamage;
        }

        public void Dispose()
        {
            _hp.Dispose();
        }
    }
}