using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Runtime.Audio;
using Runtime.Bonus;
using Runtime.Enemy;
using Runtime.GameSystem.Result;
using Runtime.GameSystem.Timer;
using Runtime.GameSystem.Tutorial;
using Runtime.GameSystem.Ranking;
using Runtime.KeyBoard;
using Runtime.Utility;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Runtime.GameSystem;

public class MainSequence : IStartable
{
    private readonly GameTimer _gameTimer;
    private readonly EnemyWaveDirector _enemyWaveDirector;
    private readonly CancellationToken _destroyCancellationToken;
    private readonly TutorialSequence _tutorialSequence;
    private readonly KeyBoardPresenter _keyBoardPresenter;
    private readonly AudioManager _audioManager;
    private readonly ResultSequence _resultSequence;
    private readonly BonusPresenter _bonusPresenter;
    private readonly RankingSequence _rankingSequence;
    public MainSequence(
        GameTimer gameTimer,
        EnemyWaveDirector enemyWaveDirector,
        TutorialSequence tutorialSequence,
        ResultSequence resultSequence,
        KeyBoardPresenter keyBoardPresenter,
        AudioManager audioManager,
        BonusPresenter bonusPresenter,
        RankingSequence rankingSequence,
        CancellationToken destroyCancellationToken)
    {
        _bonusPresenter = bonusPresenter;
        _rankingSequence = rankingSequence;
        _gameTimer = gameTimer;
        _enemyWaveDirector = enemyWaveDirector;
        _tutorialSequence = tutorialSequence;
        _resultSequence = resultSequence;
        _destroyCancellationToken = destroyCancellationToken;
        _keyBoardPresenter = keyBoardPresenter;
        _audioManager = audioManager;
    }

    void IStartable.Start()
    {
        RunAsync(_destroyCancellationToken).Forget();
    }

    /// <summary>
    /// メインシーケンスを非同期で実行する
    /// ここに条件分岐が発生するようだと責務過多となるので、条件分岐が発生する場合は別のクラスに切り出す
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async UniTaskVoid RunAsync(CancellationToken cancellationToken)
    {
        _audioManager.PlayBgmAsync("MasterBGM", cancellationToken).Forget();
        // ランキング用の名前を入力するシーケンスを実行
        await NameInputSequenceAsync(cancellationToken);
        // チュートリアルを実行
        await _tutorialSequence.StartSequenceAsync(cancellationToken);
        // ゲーム開始のシーケンスを実行
        await StartGameSequenceAsync(cancellationToken);
        // ゲーム終了後のリザルトシーケンスを実行
        await _resultSequence.StartSequenceAsync(cancellationToken);
        // ランキングのシーケンスを実行
        try
        {
            await _rankingSequence.StartSequenceAsync(cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            CustomDebug.LogError($"ランキングシーケンスの実行中にエラーが発生しました: {ex.Message}");
        }
        finally
        {
            await SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// ランキング用の名前を入力するシーケンスを実行する
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async UniTask NameInputSequenceAsync(CancellationToken cancellationToken)
    {
        // - キーボード表示
        await _keyBoardPresenter.ShowAsync(cancellationToken);
        // - 名前入力完了まで待機
        await _keyBoardPresenter.WaitForNameInputAsync(cancellationToken);
        // - キーボード非表示
        await _keyBoardPresenter.HideAsync(cancellationToken);
    }

    /// <summary>
    /// ゲーム開始のシーケンスを実行する
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async UniTask StartGameSequenceAsync(CancellationToken cancellationToken)
    {
        // ゲーム中だけ有効なトークン。タイムアップ時にキャンセルして、ゲーム中限定の演出を止める
        using var inGameCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // ゲーム開始の処理をここに記述
        _gameTimer.StartTimer();
        _bonusPresenter.Initialize(inGameCts.Token);
        _enemyWaveDirector.StartAsync(cancellationToken).Forget();
        try
        {
            await _gameTimer.OnTimeUp.FirstAsync(cancellationToken).AsUniTask();
        }
        finally
        {
            inGameCts.Cancel();
        }
    }
}