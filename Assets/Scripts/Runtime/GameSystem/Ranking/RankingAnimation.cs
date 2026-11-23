using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.GameSystem.Ranking
{
    /// <summary>
    /// ランキング画面のアニメーションを管理するクラス
    /// ランキングシステムにアタッチする予定
    /// </summary>
    public class RankingAnimation : MonoBehaviour
    {
        [SerializeField] private Image _rankingPanel = null!;
        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _fadeOutDuration = 0.5f;

        /// <summary>
        /// ランキングパネルをフェードインさせる
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async UniTask FadeInPanelAsync(CancellationToken cancellationToken)
        {
            _rankingPanel.rectTransform.localScale = Vector3.zero;
            _rankingPanel.gameObject.SetActive(true);

            try
            {
                // パネルのフェードイン処理をここに実装
                await DOTween.Sequence()
                    .Append(_rankingPanel.rectTransform.DOScale(Vector3.one, _fadeInDuration))
                    .ToUniTask(cancellationToken: cancellationToken);
            }
            finally
            {
                _rankingPanel.rectTransform.localScale = Vector3.one;
            }
        }

        public async UniTask ShowRankingAsync(GameObject rankingObject, CancellationToken cancellationToken)
        {
            rankingObject.TryGetComponent(out Image rankingImage);
            var color = rankingImage.color;
            color.a = 0f;
            rankingImage.color = color;
            rankingImage.DOFade(1f, _fadeInDuration);
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
        }

        public async UniTask FadeOutPanelAsync(CancellationToken cancellationToken)
        {
            try
            {
                // パネルのフェードアウト処理をここに実装
                await DOTween.Sequence()
                    .Append(_rankingPanel.rectTransform.DOScale(Vector3.zero, _fadeOutDuration))
                    .ToUniTask(cancellationToken: cancellationToken);
            }
            finally
            {
                _rankingPanel.gameObject.SetActive(false);
                _rankingPanel.rectTransform.localScale = Vector3.one;
            }
        }
    }
}