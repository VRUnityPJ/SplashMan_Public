using R3;
using UnityEngine;

namespace Runtime.Scores
{
    /// <summary>
    /// プレイヤーが所持するスコアを表すクラス.
    /// </summary>
    public class PlayerScore : MonoBehaviour
    {
        [SerializeField] private ScoreDto _scoreDto;

        /// <summary>
        /// 合計スコア.
        /// </summary>
        public ReadOnlyReactiveProperty<Score> TotalScore => _totalScore;

        private readonly ReactiveProperty<Score> _totalScore = new ReactiveProperty<Scores.Score>();

        /// <summary>
        /// スコアが加算されたときに、加算された量を流す.
        /// Setでの変更では流れない.
        /// </summary>
        public Observable<Score> OnAdded => _onAdded;

        private readonly Subject<Score> _onAdded = new Subject<Score>();

        /// <summary>
        /// スコアが減算されたときに、減算しようとした量を流す.
        /// スコアが0で実際には減らなかった場合も流れる. Setでの変更では流れない.
        /// </summary>
        public Observable<Score> OnSubtracted => _onSubtracted;

        private readonly Subject<Score> _onSubtracted = new Subject<Score>();

        /// <summary>
        /// スコアを加算する.
        /// </summary>
        public void Add(Score amount)
        {
            _totalScore.Value += amount;
            _onAdded.OnNext(amount);
        }

        /// <summary>
        /// スコアを減算する. 0未満にはならない.
        /// </summary>
        public void Subtract(Score amount)
        {
            _totalScore.Value = amount >= _totalScore.Value ? Score.Zero : _totalScore.Value - amount;
            _onSubtracted.OnNext(amount);
        }

        /// <summary>
        /// スコアを指定した値に設定する.
        /// </summary>
        /// <param name="amount"></param>
        public void Set(Score amount) => _totalScore.Value = amount;

        private void Start()
        {
            _totalScore.Value = _scoreDto.ToScore();
        }

        private void Update()
        {
            // UnityEditor上での動作確認用にUpdateを追加
            // #if UNITY_EDITOR
            // _totalScore.Value = _scoreDto.ToScore();
            // #endif
        }

        private void OnDestroy()
        {
            _totalScore.Dispose();
            _onAdded.Dispose();
            _onSubtracted.Dispose();
        }

#if UNITY_EDITOR
        /// <summary>
        /// デバッグ用: スコアを加算する
        /// 同じ値を指定回数連続で加算するので、同時に複数体倒したときのポップアップの積み重ねも確認できる
        /// </summary>
        [Button("スコア加算")]
        private void DebugAdd(int amount, int times)
        {
            for (var i = 0; i < Mathf.Max(1, times); i++)
            {
                Add(new Score(Mathf.Max(0, amount)));
            }
        }
#endif
    }
}