using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Bonus.Core;
using Runtime.Enemy;
using Runtime.Scores;

namespace Runtime.GameSystem.Tutorial;

/// <summary>
/// チュートリアルのシーケンスを管理するクラス
/// </summary>
public class TutorialSequence
{
    private readonly TutorialPresenter _tutorialPresenter;
    private readonly TutorialPlayerObserver _playerObserver;
    private readonly TutorialPracticeEnemy _practiceEnemy;
    private readonly PlayerScore _playerScore;
    private readonly EnemyRegistry _enemyRegistry;

    public TutorialSequence(
        TutorialPresenter tutorialPresenter,
        TutorialPlayerObserver playerObserver,
        TutorialPracticeEnemy practiceEnemy,
        PlayerScore playerScore,
        EnemyRegistry enemyRegistry)
    {
        _tutorialPresenter = tutorialPresenter;
        _playerObserver = playerObserver;
        _practiceEnemy = practiceEnemy;
        _playerScore = playerScore;
        _enemyRegistry = enemyRegistry;
    }

    /// <summary>
    /// チュートリアルのシーケンスを開始する非同期メソッド
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async UniTask StartSequenceAsync(CancellationToken cancellationToken)
    {
        _tutorialPresenter.Initialize();
        // チュートリアルのシーケンスを開始する処理をここに実装
        var select = await _tutorialPresenter.SelectStartingTutorialAsync(cancellationToken: cancellationToken);

        // チュートリアル開始を選択された時の処理
        if (select == true)
        {
            await RunTutorialAsync(cancellationToken);
        }
    }

    /// <summary>
    /// チュートリアル本体。説明を表示し、プレイヤーが実際に操作できたら次のステップへ進む
    /// </summary>
    private async UniTask RunTutorialAsync(CancellationToken cancellationToken)
    {
        // 練習中のスコアの増減を本番に持ち込まないよう、開始時点のスコアを覚えておく
        var scoreBeforeTutorial = _playerScore.TotalScore.CurrentValue;

        // - ボトルをつかむ
        _tutorialPresenter.ShowPage(TutorialStep.Catch);
        await _playerObserver.WaitForCatchAsync(cancellationToken);

        // - ボトルを振って炭酸ゲージをためる
        _tutorialPresenter.ShowPage(TutorialStep.Carbonation);
        await _playerObserver.WaitForCarbonationAsync(cancellationToken);

        // - 練習用の敵を1体出して、ボトルを投げて倒す
        _tutorialPresenter.ShowPage(TutorialStep.Practice);
        await _practiceEnemy.WaitForDefeatAsync(cancellationToken);

        // - 制限時間とスコアの説明。ボタンが押されたらゲーム開始
        _tutorialPresenter.ShowPage(TutorialStep.Score);
        await _tutorialPresenter.WaitForNextAsync(cancellationToken);
        _tutorialPresenter.HidePage();

        // 練習の結果（残った敵・スコア・獲得したボーナス）を本番に持ち込まない
        _enemyRegistry.DespawnAll();
        _playerScore.Set(scoreBeforeTutorial);
        BonusCore.Clear();
    }
}
