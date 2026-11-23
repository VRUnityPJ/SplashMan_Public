using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitMotion;
using Runtime.Bonus.Core;
using Runtime.Bonus.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Ease = LitMotion.Ease;

namespace Runtime.GameSystem.Result
{
    /// <summary>
    /// リザルト画面のアニメーションを管理するクラス
    /// </summary>
    public class ResultAnimation : MonoBehaviour
    {
        /// <summary>
        /// リザルト画面用のパネルのImageコンポーネント
        /// </summary>
        [SerializeField] private Image _resultPanel = null!;

        /// <summary>
        /// ボーナススコアの表示用の親RectTransform
        /// </summary>
        [SerializeField] private RectTransform _bonusContentParent = null!;

        /// <summary>
        /// スコアの表示用の親RectTransform
        /// </summary>
        [SerializeField] private RectTransform _scoreContentParent = null!;

        /// <summary>
        /// 合計スコア用のTextMeshProUGUIコンポーネント
        /// </summary>
        [SerializeField] private TextMeshProUGUI _resultScoreText = null!;

        [SerializeField] private TextMeshProUGUI _bonusNamePrefab = null!;
        [SerializeField] private TextMeshProUGUI _scorePrefab = null!;

        /// <summary>
        /// ボーナスのランクごとの文字色。未設定の場合は色を変えない
        /// </summary>
        [SerializeField] private BonusRankColors _bonusRankColors;

        [Header("アニメーション設定")]
        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _fadeOutDuration = 0.5f;

        /// <summary>
        /// リザルトパネルを非表示にする
        /// </summary>
        public void Hide()
        {
            _resultPanel.rectTransform.localScale = Vector3.zero;
            _resultPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// 前回表示したボーナスの行を削除する
        /// </summary>
        private void ClearBonusScores()
        {
            foreach (Transform child in _bonusContentParent) Destroy(child.gameObject);
            foreach (Transform child in _scoreContentParent) Destroy(child.gameObject);
        }

        /// <summary>
        /// 合計スコアを設定する
        /// </summary>
        /// <param name="score"></param>
        public void SetResultScore(int score) => _resultScoreText.text = score.ToString("N0");

        /// <summary>
        /// リザルトパネルをフェードインさせる非同期メソッド
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async UniTask FadeInPanelAsync(CancellationToken cancellationToken)
        {
            ClearBonusScores();
            _resultPanel.rectTransform.localScale = Vector3.zero;
            _resultPanel.gameObject.SetActive(true);

            try
            {
                // パネルのフェードイン処理をここに実装
                await DOTween.Sequence()
                    .Append(_resultPanel.rectTransform.DOScale(Vector3.one, _fadeInDuration))
                    .ToUniTask(cancellationToken: cancellationToken);
            }
            finally
            {
                _resultPanel.rectTransform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// リザルトパネルをフェードアウトさせる非同期メソッド
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async UniTask FadeOutPanelAsync(CancellationToken cancellationToken)
        {
            try
            {
                // パネルのフェードアウト処理をここに実装
                await DOTween.Sequence()
                    .Append(_resultPanel.rectTransform.DOScale(Vector3.zero, _fadeOutDuration))
                    .ToUniTask(cancellationToken: cancellationToken);
            }
            finally
            {
                _resultPanel.rectTransform.localScale = Vector3.zero;
                _resultPanel.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ボーナススコアの表示アニメーションを非同期で実行するメソッド
        /// </summary>
        /// <param name="bonusName"></param>
        /// <param name="bonusScore"></param>
        /// <param name="rank">ボーナスのランク。ランクに応じて文字色を変える</param>
        /// <param name="cancellationToken"></param>
        public async UniTask ShowBonusScoreAsync(string bonusName, int bonusScore, BonusRank rank, CancellationToken cancellationToken)
        {
            var bonusNameTexts = await InstantiateAsync(_bonusNamePrefab, _bonusContentParent.transform)
                .ToUniTask(cancellationToken: cancellationToken);
            var scoreTexts = await InstantiateAsync(_scorePrefab, _scoreContentParent.transform)
                .ToUniTask(cancellationToken: cancellationToken);
            var text = bonusNameTexts[0];
            var scoreText = scoreTexts[0];
            text.rectTransform.localPosition = Vector3.zero;
            text.rectTransform.localRotation = Quaternion.identity;
            scoreText.rectTransform.localPosition = Vector3.zero;
            scoreText.rectTransform.localRotation = Quaternion.identity;
            text.text = bonusName;
            scoreText.text = bonusScore.ToString();
            if (_bonusRankColors != null)
            {
                _bonusRankColors.ApplyTo(text, rank);
                _bonusRankColors.ApplyTo(scoreText, rank);
            }

            await DOTween.Sequence()
                .Append(text.rectTransform.DOPunchScale(Vector3.one, 0.3f))
                .Append(scoreText.rectTransform.DOPunchScale(Vector3.one, 0.3f))
                .ToUniTask(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// スコアの加算アニメーションを非同期で実行するメソッド
        /// </summary>
        /// <param name="currentScore"></param>
        /// <param name="bonusScore"></param>
        /// <param name="cancellationToken"></param>
        public async UniTask AddScoreAnimationAsync(int currentScore, int bonusScore, CancellationToken cancellationToken)
        {
            await LMotion.Create(currentScore, currentScore + bonusScore, 1f)
                .WithEase(Ease.Linear)
                .Bind(value => _resultScoreText.text = value.ToString("N0"))
                .ToUniTask(cancellationToken);
        }
    }
}
