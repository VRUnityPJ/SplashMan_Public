using System.Threading;
using Cysharp.Threading.Tasks;

namespace Runtime.Enemy
{
    /// <summary>
    /// 敵がプレイヤーに到達したときのアニメーション.
    /// EnemyCoreと同じGameObjectに付けると、到達時に再生し終わるのを待ってから敵が破棄される.
    /// 破棄はEnemyCoreが行うので、実装側では破棄しないこと.
    /// </summary>
    public interface IEnemyReachAnimation
    {
        /// <summary>
        /// 到達アニメーションを再生して、終わるまで待つ. 再生できなければすぐに終わる.
        /// </summary>
        UniTask PlayReachAsync(CancellationToken ct);
    }
}
