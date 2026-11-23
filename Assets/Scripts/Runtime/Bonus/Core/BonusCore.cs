using System;
using System.Collections.Generic;
using R3;
using Runtime.Scores;

namespace Runtime.Bonus.Core;

/// <summary>
/// ボーナスの基盤となるクラス
/// 新しいボーナスを実装するときはこれを継承して使用すること
/// </summary>
public abstract class BonusCore : IDisposable
{
    private static readonly List<BonusCore> s_earnedBonuses = new();
    private static readonly Subject<BonusCore> s_onEarned = new();

    private readonly PlayerScore _playerScore;
    private IDisposable _requireSubscription;

    /// <summary>
    /// 総合スコア用のストリーム。ボーナスの条件を登録する際に参照する想定
    /// 汎用性が高いので、BonusCoreの中で保持しておく
    /// </summary>
    protected ReadOnlyReactiveProperty<Score> TotalScore => _playerScore.TotalScore;

    /// <summary>
    /// これまでに獲得したボーナスの一覧(発生順)
    /// ゲーム終了後にまとめて表示・スコア反映する側から参照する想定
    /// </summary>
    public static IReadOnlyList<BonusCore> EarnedBonuses => s_earnedBonuses;

    /// <summary>
    /// ボーナスを獲得した瞬間に、そのボーナス自身を流すストリーム
    /// BonusPresenterはこれを購読してUI表示を更新する想定
    /// </summary>
    public static Observable<BonusCore> OnEarned => s_onEarned;

    /// <summary>
    /// ボーナスの表示名
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// このボーナスで得られるスコア
    /// </summary>
    public abstract Score Amount { get; }

    /// <summary>
    /// ボーナスのランク。獲得が難しいボーナスはoverrideしてランクを上げると、表示の色が変わる
    /// </summary>
    public virtual BonusRank Rank => BonusRank.Normal;

    protected BonusCore(PlayerScore playerScore)
    {
        _playerScore = playerScore;
    }

    /// <summary>
    /// ボーナスを獲得する条件を登録する。
    /// </summary>
    protected abstract BonusRequired Register();

    /// <summary>
    /// Requireの条件購読を開始する。
    /// コンストラクタでは仮想メンバー呼び出しを避けるため、生成後に呼び出すこと
    /// </summary>
    public void Initialize() =>
        _requireSubscription = Register()
            .Stream
            .Subscribe(this, static (_, self) =>
            {
                s_earnedBonuses.Add(self);
                s_onEarned.OnNext(self);
            });

    /// <summary>
    /// これまでに獲得したボーナスの一覧をクリアする
    /// </summary>
    public static void Clear() => s_earnedBonuses.Clear();

    void IDisposable.Dispose()
    {
        _requireSubscription?.Dispose();
        _requireSubscription = null;

        // s_onEarnedはstaticでシーンをまたいで使い回すため、ここではDisposeしない
        // (Disposeすると、シーン再読み込み後の購読がObjectDisposedExceptionになる)
    }
}
