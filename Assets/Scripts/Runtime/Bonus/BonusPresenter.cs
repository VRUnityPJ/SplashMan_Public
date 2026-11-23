using System.Threading;
using R3;
using Runtime.Bonus.Core;
using Runtime.Bonus.UI;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Bonus
{
    public class BonusPresenter : MonoBehaviour
    {
        [SerializeField] private BonusAnimation _bonusAnimation;

        /// <summary>
        /// ボーナスの表示を開始する
        /// cancelTokenがキャンセルされると購読を解除し、再生中・再生待ちのアニメーションもすべて止める
        /// </summary>
        /// <param name="cancelToken"></param>
        public void Initialize(CancellationToken cancelToken)
        {
            // 購読をDisposeすると、SubscribeAwaitに渡されるcancellationTokenがキャンセルされ、
            // 順番待ちのボーナスも破棄される
            BonusCore.OnEarned
                .SubscribeAwait(async (value, cancellationToken) =>
                {
                    CustomDebug.Log($"Bonus Earned: {value.Name}, Amount: {value.Amount}");
                    await _bonusAnimation.ShowBonusAsync(value.Name, value.Rank, cancellationToken);
                })
                .AddTo(this)
                .RegisterTo(cancelToken);
        }
    }
}
