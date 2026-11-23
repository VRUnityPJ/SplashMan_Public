using Runtime.Enemy;
using VContainer;
using VContainer.Unity;

namespace Runtime.GameSystem;

public partial class SystemLifetimeScope : LifetimeScope
{
    private void ConfigureCarbonation(IContainerBuilder builder)
    {
        // ここに炭酸関連の依存関係を登録する
        builder.RegisterComponentInHierarchy<AttackEnemy>();
    }
}