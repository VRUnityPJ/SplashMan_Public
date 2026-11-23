using Runtime.Enemy;
using Runtime.GameSystem.Timer;
using Runtime.Scores;
using Runtime.KeyBoard;
using Runtime.Utility;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.GameSystem
{
    /// <summary>
    /// システム全体で使う依存関係を登録するLifetimeScope
    /// PlayerやEnemyなどのゲームオブジェクトに依存するものは、新しいSystemLifetimeScopeを作って登録する
    /// partialにしてあるので、新規にファイルを追加するときはSystemLifetimeScope.Player.csのように名前をつけてpartial classを作ると、依存関係の登録を分けることができる
    /// これでコンフリクトのリスクを減らすことができる
    /// </summary>
    public partial class SystemLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ここにシステム全体で使う依存関係を登録する
            builder.RegisterEntryPoint<MainSequence>();

            builder.RegisterInstance(destroyCancellationToken);

            builder.RegisterComponentInHierarchy<GameTimer>();
            builder.RegisterComponentInHierarchy<PlayerScore>();
            builder.RegisterComponentInHierarchy<KeyBoardPresenter>();
            builder.RegisterComponentInHierarchy<EnemyRegistry>();
            builder.RegisterComponentInHierarchy<EnemyWaveDirector>();

            ConfigureAudio(builder);
            ConfigureBonus(builder);
            ConfigureCarbonation(builder);
            ConfigureResult(builder);
            ConfigureTutorial(builder);
            ConfigureRanking(builder);

            builder.RegisterEntryPointExceptionHandler(ex =>
            {
                CustomDebug.LogError($"Exception in MainSequence: {ex}");
            });
        }
    }
}