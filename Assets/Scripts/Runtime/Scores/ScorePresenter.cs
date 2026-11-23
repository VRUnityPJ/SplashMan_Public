using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;

namespace Runtime.Scores
{
    /// <summary>
    /// プレイ中のスコアを表示するためのプレゼンター
    /// </summary>
    public class ScorePresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private PlayerScore _playerScore;
        [SerializeField] private ScorePopupView _scorePopupView;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            // クロージャ発生させないようにしている
            _playerScore.TotalScore
                .Subscribe(this, static (score, self) =>
                {
                    self.UpdateScoreText(score);
                })
                .AddTo(this);

            // ポップアップは同時に複数表示するので、完了を待たずに次々出す
            _playerScore.OnAdded
                .Subscribe(this, static (amount, self) =>
                {
                    self._scorePopupView.ShowAsync($"+{amount.Value}", self.destroyCancellationToken).Forget();
                })
                .AddTo(this);
        }

        private void UpdateScoreText(Score score)
        {
            _scoreText.text = $"Score: {score.Value:N0}";
        }
    }
}