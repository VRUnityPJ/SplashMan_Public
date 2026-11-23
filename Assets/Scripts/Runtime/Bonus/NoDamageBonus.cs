using R3;
using Runtime.Bonus.Core;
using Runtime.GameSystem.Timer;
using Runtime.Scores;

namespace Runtime.Bonus;

/// <summary>
/// ゲーム中に一度も敵の到達を許さず(スコアを減らされず)にタイムアップを迎えた時に得られるボーナス
/// </summary>
public class NoDamageBonus : BonusCore
{
    private readonly PlayerScore _playerScore;
    private readonly GameTimer _gameTimer;

    public NoDamageBonus(PlayerScore playerScore, GameTimer gameTimer) : base(playerScore)
    {
        _playerScore = playerScore;
        _gameTimer = gameTimer;
    }

    /// <inheritdoc />
    public override string Name => "完全勝利ボーナス";

    /// <inheritdoc />
    public override Score Amount { get; } = new(50000);

    /// <inheritdoc />
    public override BonusRank Rank => BonusRank.Rare;

    /// <inheritdoc />
    protected override BonusRequired Register()
    {
        // ゲーム中にダメージを受けたかどうか。最初は受けていない(false)状態から始める
        // タイマーが動いていない間(チュートリアルの練習など)のダメージは数えない
        var isDamaged = _playerScore.OnSubtracted
            .Where(_ => _gameTimer.IsRunning)
            .Select(_ => true)
            .Prepend(false);

        // ゲームが終わるまで達成が確定しないので、タイムアップの瞬間に判定する
        return BonusTrigger
            .Watch(_gameTimer.OnTimeUp.WithLatestFrom(isDamaged, (_, damaged) => damaged))
            .When(damaged => !damaged);
    }
}
