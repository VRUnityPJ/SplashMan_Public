using TMPro;
using UnityEngine;

namespace Runtime.GameSystem.Ranking
{
    /// <summary>
    /// ランキング画面のスロットにデータを設定するクラス
    /// ランキングのスロットプレハブにアタッチする予定
    /// </summary>
    public class SetRankingSlot : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _rankingNumberText;

        [SerializeField]
        private TextMeshProUGUI _nameText;

        [SerializeField]
        private TextMeshProUGUI _scoreText;

        public void SetData(int i, string playerName, int score)
        {
            _rankingNumberText.text = i.ToString();
            _nameText.text = playerName;
            _scoreText.text = score.ToString();
        }
    }
}