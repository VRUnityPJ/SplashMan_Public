using System;
using R3;

namespace Runtime.Bonus.Core;

/// <summary>
/// ボーナス獲得の条件を表すクラス。
/// </summary>
public sealed class BonusRequired
{
    /// <summary>
    /// BonusTriggerから生成したストリーム。
    /// BonusCoreはこのストリームを監視し、条件が満たされたらボーナスを付与する。
    /// </summary>
    public Observable<Unit> Stream { get; }

    public BonusRequired(Observable<Unit> stream) => Stream = stream;
}

/// <summary>
/// ボーナス獲得の条件を生成するためのクラス。
/// </summary>
public static class BonusTrigger
{
    /// <summary>
    /// 指定したストリームを監視する。
    /// </summary>
    /// <param name="stream"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Trigger<T> Watch<T>(Observable<T> stream) => new(stream);

    /// <summary>
    /// 指定したストリームを監視する。
    /// </summary>
    /// <param name="stream"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Trigger<T> Watch<T>(ReadOnlyReactiveProperty<T> stream) => new(stream);

    /// <summary>
    /// BonusRequiredを生成するための中間クラス。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Trigger<T>
    {
        private readonly Observable<T> _stream;

        /// <summary>
        ///　本当はTriggerのコンストラクタをprivateにしたいが、C#の仕様上、無理なのでpublicにしている。
        /// </summary>
        /// <param name="stream"></param>
        public Trigger(Observable<T> stream) => _stream = stream;

        /// <summary>
        /// ボーナスを獲得する条件を指定する。
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public BonusRequired When(Func<T, bool> condition)
        {
            var triggerStream = _stream
                .Where(condition)
                .Select(_ => Unit.Default);

            return new BonusRequired(triggerStream);
        }
    }
}
