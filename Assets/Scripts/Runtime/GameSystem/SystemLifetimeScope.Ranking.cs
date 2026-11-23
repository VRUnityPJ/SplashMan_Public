using Runtime.GameSystem.Ranking;
using VContainer;
using VContainer.Unity;

namespace Runtime.GameSystem;

public partial class SystemLifetimeScope : LifetimeScope
{
    private void ConfigureRanking(IContainerBuilder builder)
    {
        // ランキングの処理を登録
        builder.Register<RankingPlayFab>(Lifetime.Singleton);
        builder.Register<RankingSequence>(Lifetime.Singleton);

        // シーン上に配置されているランキング表示コンポーネントを登録
        builder.RegisterComponentInHierarchy<RankingPresenter>();
    }
}