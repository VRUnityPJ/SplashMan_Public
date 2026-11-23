using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Bonus.Core;
using Runtime.Scores;

namespace Runtime.GameSystem.Result;

/// <summary>
/// リザルト画面のシーケンスを管理するクラス
/// </summary>
public class ResultSequence
{
    private readonly ResultPresenter _resultPresenter;
    private readonly PlayerScore _playerScore;

    public ResultSequence(ResultPresenter resultPresenter, PlayerScore playerScore)
    {
        _resultPresenter = resultPresenter;
        _playerScore = playerScore;
    }

    /// <summary>
    /// リザルト画面を表示するシーケンスを開始する
    /// ボーナスの計算やスコアの集計を行い、結果を表示する
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async UniTask StartSequenceAsync(CancellationToken cancellationToken)
    {
        var result = GameResult.Create(_playerScore.TotalScore.CurrentValue, BonusCore.EarnedBonuses);
        // リザルト画面のシーケンスを開始する処理をここに実装
        await _resultPresenter.ShowResultAsync(result, cancellationToken: cancellationToken);
        // リザルト画面の表示が完了したら、最終スコアをプレイヤーのスコアに反映する
        _playerScore.Set(result.FinalScore);

        // ボーナスの計算が終わったら、ボーナスの状態をクリアする
        BonusCore.Clear();
    }
}