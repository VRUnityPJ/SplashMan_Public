using System.Collections.Generic;
using VContainer.Unity;

namespace Runtime.Bonus.Core;

/// <summary>
/// ボーナスの初期化を行う専用クラス
/// </summary>
public sealed class BonusInitializer : IStartable
{
    private readonly IReadOnlyList<BonusCore> _bonuses;

    public BonusInitializer(IReadOnlyList<BonusCore> bonuses) => _bonuses = bonuses;

    void IStartable.Start()
    {
        foreach (var bonusCore in _bonuses)
        {
            bonusCore.Initialize();
        }
    }
}