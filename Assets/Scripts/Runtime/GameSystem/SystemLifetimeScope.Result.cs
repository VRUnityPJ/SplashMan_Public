using Runtime.Enemy;
using Runtime.GameSystem.Result;
using VContainer;
using VContainer.Unity;

namespace Runtime.GameSystem;

public partial class SystemLifetimeScope : LifetimeScope
{
    private void ConfigureResult(IContainerBuilder builder)
    {
        // ここに炭酸関連の依存関係を登録する
        builder.Register<ResultSequence>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<ResultPresenter>();
    }
}