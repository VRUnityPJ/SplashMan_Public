using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Enemy
{
    /// <summary>
    /// 到達アニメーションの仮実装.
    /// 正式なEnemyAnimationができたら、そちらにIEnemyReachAnimationを実装して差し替える.
    /// </summary>
    public class TempEnemyReachAnimation : MonoBehaviour, IEnemyReachAnimation
    {
        [SerializeField] private Animator _animator;

        /// <summary>
        /// 到達時に再生するTrigger名. Animatorのステート名も同じ名前にすること.
        /// </summary>
        [SerializeField] private string _reachName = "Reach";

        private int _reachHash;

        /// <summary>
        /// 到達アニメーションを再生できるか(Animatorがあり、Triggerが存在するか).
        /// </summary>
        private bool _canPlay;

        private void Awake()
        {
            _reachHash = Animator.StringToHash(_reachName);
            _canPlay = _animator != null && HasTrigger(_animator, _reachHash);
        }

        public async UniTask PlayReachAsync(CancellationToken ct)
        {
            if (!_canPlay) return;

            _animator.SetTrigger(_reachHash);

            // Reachステートに入るまで待つ
            await UniTask.WaitUntil(
                () => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _reachHash,
                cancellationToken: ct);

            // 最後まで再生されるまで待つ
            await UniTask.WaitUntil(
                () => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f,
                cancellationToken: ct);
        }

        private static bool HasTrigger(Animator animator, int hash)
        {
            foreach (var parameter in animator.parameters)
            {
                if (parameter.nameHash == hash && parameter.type == AnimatorControllerParameterType.Trigger)
                    return true;
            }
            return false;
        }
    }
}
