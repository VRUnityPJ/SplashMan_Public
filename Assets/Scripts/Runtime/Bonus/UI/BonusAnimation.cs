using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Bonus.Core;
using TMPro;
using UnityEngine;

namespace Runtime.Bonus.UI
{
    public class BonusAnimation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _bonusText;
        [SerializeField] private RectTransform _spawnPoint;
        [SerializeField] private Vector3 _spawnPosition;
        [SerializeField, Tooltip("ボーナスのランクごとの文字色。未設定の場合は色を変えない")]
        private BonusRankColors _rankColors;

        [Header("アニメーション設定")]
        [SerializeField] private float _scaleDuration = 0.5f;
        [SerializeField] private Vector3 _rotationAngle = new Vector3(0, 0, 360f);
        [SerializeField] private float _rotationDuration = 0.1f;
        [SerializeField] private float _showDuration = 1.0f;

        /// <summary>
        /// ボーナス名を表示するアニメーションを実行する
        /// </summary>
        /// <param name="bonusName"></param>
        /// <param name="rank">ボーナスのランク。ランクに応じて文字色を変える</param>
        /// <param name="cancellationToken"></param>
        public async UniTask ShowBonusAsync(string bonusName, BonusRank rank, CancellationToken cancellationToken)
        {
            var texts = await InstantiateAsync(_bonusText, _spawnPoint)
                .ToUniTask(cancellationToken: cancellationToken);
            var obj = texts[0];
            try
            {
                obj.text = bonusName;
                if (_rankColors != null)
                {
                    _rankColors.ApplyTo(obj, rank);
                }
                obj.transform.localScale = Vector3.zero;
                obj.rectTransform.localRotation = Quaternion.identity;
                obj.rectTransform.localPosition = _spawnPosition;

                await DOTween.Sequence()
                    .Append(obj.transform.DOScale(Vector3.one, _scaleDuration))
                    .Join(obj.rectTransform.DOLocalRotate(_rotationAngle, _rotationDuration).SetRelative())
                    .ToUniTask(cancellationToken: cancellationToken);

                await UniTask.Delay(TimeSpan.FromSeconds(_showDuration), cancellationToken: cancellationToken);

                await DOTween.Sequence()
                    .Append(obj.transform.DOScale(Vector3.zero, _scaleDuration))
                    .Join(obj.rectTransform.DOLocalRotate(_rotationAngle, _rotationDuration).SetRelative())
                    .ToUniTask(cancellationToken: cancellationToken);
            }
            finally
            {
                Destroy(obj.gameObject);
            }
        }

#if UNITY_EDITOR
        private CancellationTokenSource _debugCancellationTokenSource;

        /// <summary>
        /// デバッグ用: ダミーの結果でリザルトの演出を再生する
        /// 再生中に押した場合は、前の再生をキャンセルして最初からやり直す
        /// </summary>
        [Button("ダミーデータでリザルト再生")]
        private void DebugShowBonus(string bonusName, BonusRank rank)
        {
            _debugCancellationTokenSource?.Cancel();
            _debugCancellationTokenSource = new CancellationTokenSource();
            ShowBonusAsync(bonusName, rank, _debugCancellationTokenSource.Token).Forget();
        }
#endif
    }
}
