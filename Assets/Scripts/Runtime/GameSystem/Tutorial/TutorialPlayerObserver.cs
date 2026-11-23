using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Runtime.Bottle;
using Runtime.PlayerAction;
using UnityEngine;

namespace Runtime.GameSystem.Tutorial
{
    /// <summary>
    /// プレイヤーの操作を監視して、チュートリアルの各ステップを達成したかを判定するクラス
    /// </summary>
    public class TutorialPlayerObserver : MonoBehaviour
    {
        /// <summary>
        /// 監視する手（左右両方を登録する）
        /// </summary>
        [SerializeField] private CatchThrowBottle[] _hands = null!;

        /// <summary>
        /// 炭酸ゲージのステップを達成とみなす割合（最大値に対する現在値）
        /// </summary>
        [SerializeField, Range(0f, 1f)] private float _requiredCarbonationRate = 0.5f;

        /// <summary>
        /// それぞれの手が最後につかんだボトル。インデックスは_handsに対応する
        /// </summary>
        private BottleCarbonationStatus[] _caughtBottles;

        private void Start()
        {
            _caughtBottles = new BottleCarbonationStatus[_hands.Length];
            for (var i = 0; i < _hands.Length; i++)
            {
                var index = i;
                _hands[i].OnTriggerEnterBottle
                    .Subscribe(bottle => _caughtBottles[index] = bottle.GetComponent<BottleCarbonationStatus>())
                    .AddTo(this);
            }
        }

        /// <summary>
        /// どちらかの手でボトルをつかむまで待機する
        /// </summary>
        public UniTask WaitForCatchAsync(CancellationToken cancellationToken)
            => UniTask.WaitUntil(IsHoldingAny, cancellationToken: cancellationToken);

        /// <summary>
        /// 持っているボトルの炭酸ゲージが一定の割合以上たまるまで待機する
        /// </summary>
        public UniTask WaitForCarbonationAsync(CancellationToken cancellationToken)
            => UniTask.WaitUntil(IsCarbonationCharged, cancellationToken: cancellationToken);

        private bool IsHoldingAny()
        {
            foreach (var hand in _hands)
            {
                if (hand.IsHolding) return true;
            }

            return false;
        }

        private bool IsCarbonationCharged()
        {
            for (var i = 0; i < _hands.Length; i++)
            {
                var bottle = _caughtBottles[i];
                if (!_hands[i].IsHolding || bottle == null) continue;

                if (bottle.CurrentCarbonation.Value >= bottle.MaxCarbonation.Value * _requiredCarbonationRate)
                    return true;
            }

            return false;
        }
    }
}
