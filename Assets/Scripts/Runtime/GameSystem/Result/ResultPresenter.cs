using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Audio;
using Runtime.Bonus;
using Runtime.Bonus.Core;
using Runtime.Scores;
using UnityEngine;
using VContainer;
using AudioType = Runtime.Audio.AudioType;

namespace Runtime.GameSystem.Result
{
    /// <summary>
    /// リザルト画面の表示を管理するクラス
    /// </summary>
    public class ResultPresenter : MonoBehaviour
    {
        [SerializeField] private ResultAnimation _resultAnimation;

        private AudioManager _audio;

        [Inject]
        private void Construct(AudioManager audioManager) => _audio = audioManager;

        public void Start() => Initialize();


        /// <summary>
        /// リザルト画面の初期化処理
        /// </summary>
        public void Initialize()
        {
            _resultAnimation.Hide();
        }

        public async UniTask ShowResultAsync(GameResult result, CancellationToken cancellationToken)
        {
            // リザルト画面表示
            _audio.PlaySeAsync(AudioType.Result.TimeUp, cancellationToken).Forget();
            _resultAnimation.SetResultScore(result.BaseScore.Value);
            await _resultAnimation.FadeInPanelAsync(cancellationToken);

            // 演出上、スコア表示の前に1秒待機する
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
            var currentScore = result.BaseScore.Value;

            // ボーナスの種類ごとにスコアを加算して表示する
            foreach (var bonus in result.Bonuses)
            {
                var bonusScore = bonus.TotalAmount.Value;

                _audio.PlaySeAsync(AudioType.Result.ShowBonus, cancellationToken).Forget();
                UniTask.Create(async ct =>
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: ct);
                    _audio.PlaySeAsync(AudioType.Result.ShowBonus, ct).Forget();
                }, cancellationToken).Forget();

                await _resultAnimation.ShowBonusScoreAsync(FormatBonusName(bonus.Name, bonus.Count), bonusScore, bonus.Rank, cancellationToken);

                _audio.PlaySeAsync(AudioType.Result.ScoreUp, cancellationToken).Forget();
                await _resultAnimation.AddScoreAnimationAsync(currentScore, bonusScore, cancellationToken);

                currentScore += bonusScore;
            }
            //　リザルト画面の表示が完了したら、3秒待機してからフェードアウトする
            await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: cancellationToken);
            await _resultAnimation.FadeOutPanelAsync(cancellationToken);
        }

        /// <summary>
        /// 複数回獲得したボーナスは「ボーナス名×獲得回数」の形式にする
        /// </summary>
        /// <param name="bonusName"></param>
        /// <param name="count"></param>
        private static string FormatBonusName(string bonusName, int count)
            => count > 1 ? $"{bonusName}　×{count}" : bonusName;

#if UNITY_EDITOR
        private CancellationTokenSource _debugCancellationTokenSource;

        /// <summary>
        /// デバッグ用: ダミーの結果でリザルトの演出を再生する
        /// 再生中に押した場合は、前の再生をキャンセルして最初からやり直す
        /// </summary>
        [Button("ダミーデータでリザルト再生")]
        private void DebugShowResult(int baseScore, int bonusKinds, int earnedCount)
        {
            _debugCancellationTokenSource?.Cancel();
            _debugCancellationTokenSource?.Dispose();
            _debugCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            var bonuses = new BonusSummary[Mathf.Max(0, bonusKinds)];
            var count = Mathf.Max(1, earnedCount);
            for (var i = 0; i < bonuses.Length; i++)
            {
                // ランクごとの見た目も確認できるよう、最後の1つだけレアにする
                var rank = i == bonuses.Length - 1 ? BonusRank.Rare : BonusRank.Normal;
                bonuses[i] = new BonusSummary($"デバッグボーナス{i + 1}", count, new Score(500 * count), rank);
            }

            var result = new GameResult(new Score(Mathf.Max(0, baseScore)), bonuses);
            ShowResultAsync(result, _debugCancellationTokenSource.Token).Forget();
        }
#endif
    }
}
