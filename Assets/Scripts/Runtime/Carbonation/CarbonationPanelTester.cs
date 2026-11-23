using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Bottle;
using Runtime.Carbonation.Presenter;
using UnityEngine;

namespace Runtime.Carbonation
{
    /// <summary>
    /// テスト用: BottleCarbonationInfoのリストを一定間隔で順番にPresenterへ代入する。
    /// リストの最後まで代入したら、最初には戻らず終了する。
    /// </summary>
    public class CarbonationPanelTester : MonoBehaviour
    {
        [SerializeField, Tooltip("代入先のPresenter")]
        private CarbonationPanelPresenter _presenter;

        [SerializeField, Tooltip("順番に代入していくBottleCarbonationInfoのリスト")]
        private List<BottleCarbonationStatus> _bottleCarbonationInfos;

        [SerializeField, Tooltip("代入の間隔(秒)")]
        private float _interval = 5f;

        // _bottleCarbonationInfoがprivateフィールドのため、リフレクションで直接書き換える
        private static readonly FieldInfo s_bottleInfoField =
            typeof(CarbonationPanelPresenter).GetField(
                "_bottleCarbonationInfo",
                BindingFlags.NonPublic | BindingFlags.Instance);

        private void Start()
            => FeedAsync(this.GetCancellationTokenOnDestroy()).Forget();

        private async UniTaskVoid FeedAsync(CancellationToken token)
        {
            if (_bottleCarbonationInfos == null || _bottleCarbonationInfos.Count == 0)
            {
                Debug.LogWarning($"{nameof(CarbonationPanelTester)}: リストが空です。");
                return;
            }

            foreach (var bottleInfo in _bottleCarbonationInfos)
            {
                s_bottleInfoField.SetValue(_presenter, bottleInfo);

                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(_interval),
                    cancellationToken: token);
            }
        }
    }
}