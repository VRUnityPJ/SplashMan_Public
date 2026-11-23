using Runtime.GameSystem.Tutorial;
using VContainer;
using VContainer.Unity;

namespace Runtime.GameSystem;

public partial class SystemLifetimeScope : LifetimeScope
{
    private void ConfigureTutorial(IContainerBuilder builder)
    {
        // ここにチュートリアル関連の依存関係を登録する
        builder.Register<TutorialSequence>(Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<TutorialPresenter>();
        builder.RegisterComponentInHierarchy<TutorialPlayerObserver>();
        builder.RegisterComponentInHierarchy<TutorialPracticeEnemy>();
    }
}
