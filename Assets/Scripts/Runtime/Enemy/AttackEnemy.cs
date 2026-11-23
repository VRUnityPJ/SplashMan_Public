using Runtime.Carbonation;
using UnityEngine;
using R3;
using Runtime.Bottle;
using Runtime.Utility;

namespace Runtime.Enemy
{
    public class AttackEnemy : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private int _numDamageableEnemy = 10;
        [SerializeField] private Transform _playerTransform;

        private readonly Subject<AttackResult> _attackedSubject = new();
        private readonly Subject<float> _defeatDistanceSubject = new();

        public Observable<AttackResult> OnAttacked => _attackedSubject;
        /// <summary>
        /// 敵を倒したときのプレイヤーからの距離を流す
        /// </summary>
        public Observable<float> OnDefeatDistance => _defeatDistanceSubject;

        private void Awake()
            => Subscribe();

        private void Subscribe()
        {
            BottleCollision.BottleCollisionToAttack
                .Subscribe(info =>
                {
                    var (currentCarbonation, maxCarbonation, bottlePosition) = info;
                    DamageEnemy(currentCarbonation, maxCarbonation, bottlePosition);
                }).AddTo(this);
        }

        private void DamageEnemy(Carbonation.Carbonation currentCarbonation, Carbonation.Carbonation maxCarbonation, Vector3 bottlePosition)
        {
            var rangeRadius = CalculateRange.Calculate(currentCarbonation);
            Collider[] colliders = new Collider[_numDamageableEnemy];
            //範囲内にある敵のコライダーを取得
            var attackCount = Physics.OverlapSphereNonAlloc(bottlePosition, rangeRadius, colliders, _enemyLayerMask);
            var defeatCount = 0;
            if (attackCount > 0)
            {
                for (var i = 0; i < attackCount; i++)
                {
                    var target = colliders[i];
                    if (!target.TryGetComponent<EnemyCore>(out var enemyCore))
                    {
                        continue;
                    }

                    var onDefeated = enemyCore.OnDefeated
                        .Subscribe(defeatInfo =>
                        {
                            var enemyPosition = defeatInfo.DeadPosition;
                            var distance = Vector3.Distance(_playerTransform.position, enemyPosition);
                            _defeatDistanceSubject.OnNext(distance);
                        });

                    var damage = CalculateEnemyDamage(currentCarbonation, bottlePosition, target.transform.position, rangeRadius);
                    if (enemyCore.ApplyDamage(damage))
                    {
                        defeatCount++;
                    }
                    CustomDebug.Log($"Hit Collider : {target.name}");
                    CustomDebug.Log($"Damage       : {damage}");
                    onDefeated.Dispose();
                }
                var result = new AttackResult(currentCarbonation, maxCarbonation, defeatCount, attackCount);
                _attackedSubject.OnNext(result);
            }
        }

        private int CalculateEnemyDamage(Carbonation.Carbonation carbonation, Vector3 bottlePosition, Vector3 enemyPosition, float rangeRadius)
        {
            // 炭酸ゲージが0のときは計算式だと0になるので、距離によらず固定のダメージにする
            if (carbonation.Value <= 0f)
            {
                return CalculateDamage.ZeroCarbonationDamage;
            }

            var centerDamage = CalculateDamage.CenterAttackAmount(carbonation);
            var distance = Vector3.Distance(bottlePosition, enemyPosition);
            var damage = CalculateDamage.AreaAttackAmount(centerDamage, rangeRadius, distance);
            return damage;
        }

#if UNITY_EDITOR
        /// <summary>
        /// デバッグ用: 攻撃結果を直接流す
        /// ボーナスの獲得条件やインゲームの演出を確認するときに使う
        /// </summary>
        [Button("攻撃結果を流す")]
        private void DebugEmitAttackResult(float carbonation, float maxCarbonation, int defeatedCount, int attackCount)
        {
            var result = new AttackResult(
                new Carbonation.Carbonation(carbonation),
                new Carbonation.Carbonation(maxCarbonation),
                defeatedCount,
                attackCount);
            _attackedSubject.OnNext(result);
        }
#endif
    }
}