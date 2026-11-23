using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using uPools;

namespace Runtime.Scores
{
    /// <summary>
    /// 獲得したスコアをポップアップで表示するView
    /// 右から左へフェードインしながら移動 → 少し待機 → 上へ移動しながらフェードアウト
    /// 表示中に新しいポップアップが出たら、表示中のものを上へずらして積み重ねる
    /// </summary>
    public class ScorePopupView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _popupPrefab = null!;

        /// <summary>
        /// ポップアップを生成する親。スコアUIと同じCanvas内に置く
        /// </summary>
        [SerializeField] private RectTransform _popupRoot = null!;

        /// <summary>
        /// ポップアップが止まる位置(_popupRoot内のanchoredPosition)
        /// </summary>
        [SerializeField] private Vector2 _basePosition = Vector2.zero;

        [Header("フェードイン")]
        [SerializeField] private float _slideDistance = 80f;
        [SerializeField] private float _slideInDuration = 0.25f;
        [SerializeField] private Ease _slideInEase = Ease.OutCubic;

        [Header("待機")]
        [SerializeField] private float _waitDuration = 0.4f;

        [Header("フェードアウト")]
        [SerializeField] private float _riseDistance = 40f;
        [SerializeField] private float _fadeOutDuration = 0.4f;
        [SerializeField] private Ease _fadeOutEase = Ease.InCubic;

        [Header("積み重ね")]
        [SerializeField] private float _stackSpacing = 40f;
        [SerializeField] private float _stackMoveDuration = 0.15f;

        private readonly List<RectTransform> _activePopups = new();
        private GameObjectPool _pool;

        private void Awake() => _pool = new GameObjectPool(_popupPrefab.gameObject);

        /// <summary>
        /// ポップアップを表示し、消えるまで待機する
        /// </summary>
        /// <param name="text">表示する文字列</param>
        /// <param name="cancellationToken"></param>
        public async UniTask ShowAsync(string text, CancellationToken cancellationToken)
        {
            var popupObject = _pool.Rent(_popupRoot);
            popupObject.TryGetComponent<TextMeshProUGUI>(out var popup);
            var rectTransform = popup.rectTransform;

            // プールから取り出したものは前回の状態が残っているので、開始状態を毎回作り直す
            popup.text = text;
            popup.alpha = 0f;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = new Vector3(_basePosition.x + _slideDistance, _basePosition.y, 0f);

            // 表示中のポップアップを上にずらして場所を空ける
            // Blendableを使うことで、各ポップアップ自身の移動アニメーションと重ねがけできる
            foreach (var activePopup in _activePopups)
            {
                activePopup.DOBlendableLocalMoveBy(Vector3.up * _stackSpacing, _stackMoveDuration).SetEase(Ease.OutCubic);
            }
            _activePopups.Add(rectTransform);

            try
            {
                await DOTween.Sequence()
                    .Append(rectTransform.DOBlendableLocalMoveBy(Vector3.left * _slideDistance, _slideInDuration).SetEase(_slideInEase))
                    .Join(popup.DOFade(1f, _slideInDuration))
                    .AppendInterval(_waitDuration)
                    .Append(rectTransform.DOBlendableLocalMoveBy(Vector3.up * _riseDistance, _fadeOutDuration).SetEase(_fadeOutEase))
                    .Join(popup.DOFade(0f, _fadeOutDuration))
                    .ToUniTask(cancellationToken: cancellationToken);
            }
            finally
            {
                _activePopups.Remove(rectTransform);

                // シーン破棄によるキャンセルの場合は、すでに破棄されていることがある
                if (popup != null)
                {
                    rectTransform.DOKill();
                    popup.DOKill();
                    _pool.Return(popupObject);
                }
            }
        }
    }
}
