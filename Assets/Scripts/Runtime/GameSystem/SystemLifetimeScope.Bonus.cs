using Runtime.Bonus;
using Runtime.Bonus.Core;
using VContainer;
using VContainer.Unity;

namespace Runtime.GameSystem;

public partial class SystemLifetimeScope : LifetimeScope
{
    private void ConfigureBonus(IContainerBuilder builder)
    {
        // ここにボーナス関連の依存関係を登録する
        builder.RegisterComponentInHierarchy<BonusPresenter>();
        builder.RegisterEntryPoint<BonusInitializer>();

        builder.Register<MaxCarbonationDefeatBonus>(Lifetime.Singleton)
            .As<BonusCore>();
        builder.Register<EnemyNumInRangeBonus>(Lifetime.Singleton)
            .As<BonusCore>();
        builder.Register<EnemyDefeatDistanceBonus>(Lifetime.Singleton)
            .As<BonusCore>();
        builder.Register<NoDamageBonus>(Lifetime.Singleton)
            .As<BonusCore>();
        builder.Register<ZeroCarbonationDefeatBonus>(Lifetime.Singleton)
            .As<BonusCore>();
    }
}