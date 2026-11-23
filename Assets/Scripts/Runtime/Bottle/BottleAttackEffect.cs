using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Carbonation;
using UnityEngine;
using uPools;
using R3;

namespace Runtime.Bottle
{
    /// <summary>
    /// 攻撃時にエフェクトを発生させるクラス
    /// </summary>
    public class BottleAttackEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _attackEffect;

        private void Start()
        {
            SharedGameObjectPool.Prewarm(_attackEffect.gameObject, 5);

            BottleCollision.BottleCollisionToAttack
                .Subscribe(this,static (value, self) =>
                {
                    var (currentCarbonation, maxCarbonation, position) = value;
                    var range = CalculateRange.Calculate(currentCarbonation);
                    self.PlayAsync(position, range, self.destroyCancellationToken).Forget();
                })
                .AddTo(this);
        }

        /// <summary>
        /// 攻撃エフェクトを再生する
        /// </summary>
        public async UniTask PlayAsync(Vector3 positon, float scale, CancellationToken cancellationToken)
        {
            var obj = SharedGameObjectPool.Rent(_attackEffect);
            obj.transform.position = positon;
            obj.transform.localScale = Vector3.one * scale * 0.15f;
            obj.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(_attackEffect.main.duration), cancellationToken: cancellationToken);
            SharedGameObjectPool.Return(obj.gameObject);
        }
    }
}
