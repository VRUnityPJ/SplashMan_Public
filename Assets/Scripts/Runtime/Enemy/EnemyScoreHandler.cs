using Runtime.Scores;
using UnityEngine;
using VContainer;

namespace Runtime.Enemy
{
    /// <summary>
    /// 敵に関するスコアの増減をPlayerScoreに反映するクラス.
    /// EnemyCoreから呼ばれる.
    /// </summary>
    public class EnemyScoreHandler : MonoBehaviour
    {
        private PlayerScore _playerScore;

        [Inject]
        public void Construct(PlayerScore playerScore) => _playerScore = playerScore;

        /// <summary>
        /// スコアを加算する.
        /// </summary>
        public void Add(Score score)
        {
            _playerScore.Add(score);
        }

        /// <summary>
        /// スコアを減算する. 0未満にはならない.
        /// </summary>
        public void Subtract(Score score)
        {
            _playerScore.Subtract(score);
        }
    }
}
