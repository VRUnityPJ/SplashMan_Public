using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using Runtime.Bottle;
using UnityEngine;

namespace Runtime.PlayerAction
{
    /// <summary>
    /// ボトルを取るとき、投げるときの処理を行うクラス
    /// </summary>
    public class CatchThrowBottle : MonoBehaviour
    {
        [Header("ボトルを持つときの調整")]
        [SerializeField] private float _throwAngle;
        [SerializeField] private Quaternion _catchAngle;
        [SerializeField] private Vector3 _catchPosition;
        private Transform _handTransform;
        private Collider _handCollider;
        private Collider _collider;
        private Rigidbody _bottleRb;
        private readonly Subject<Unit> _onReleased = new Subject<Unit>();
        private readonly Subject<Collider> _onTriggerEnterBottle = new Subject<Collider>();

        public bool IsHolding { get; private set; }
        public Observable<Unit> OnReleased => _onReleased;
        public Observable<Collider> OnTriggerEnterBottle => _onTriggerEnterBottle;

        private void Awake()
        {
            _onReleased.AddTo(this);
            _onTriggerEnterBottle.AddTo(this);
            TryGetComponent(out _handTransform);
            TryGetComponent(out _handCollider);
        }

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.TryGetComponent<BottleCollision>(out var bottleObject);
            if (other.CompareTag("Bottle") && !IsHolding && !bottleObject.IsCaught)
            {
                _onTriggerEnterBottle.OnNext(other);
            }
        }

        private async UniTask CatchAnimationAsync(GameObject bottle,Transform handTransform , CancellationToken cancellationToken)
        {
            await bottle.transform.DOMove(handTransform.position, 0.2f).SetEase(Ease.InExpo)
                .ToUniTask(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// ボトルを手に持つ処理
        /// </summary>
        /// <param name="other"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask<bool> CatchBottleAsync(Collider other, CancellationToken token)
        {
            if (!other.CompareTag("Bottle") || IsHolding)
            {
                return false;
            }
            other.enabled = false;
            _handCollider.enabled = false;
            other.TryGetComponent(out _bottleRb);
            _collider = other;
            _bottleRb.isKinematic = true;
            _bottleRb.useGravity = false;
            _collider.transform.SetParent(_handTransform);
            await CatchAnimationAsync(_collider.gameObject, _handTransform, token);
            _collider.transform.localRotation = _catchAngle;
            _collider.transform.localPosition = _catchPosition;
            IsHolding = true;
            _bottleRb.freezeRotation = true;
            other.TryGetComponent(out BottleCollision bottleCollision);
            bottleCollision.IsCaught = true;
            return true;
        }

        /// <summary>
        /// ボトルを手から離して投げる処理
        /// </summary>
        /// <param name="goalPosition"></param>
        /// <returns></returns>
        public bool ReleaseAndThrowBottle(Vector3 goalPosition)
        {
            if (!IsHolding)
            {
                return false;
            }
            _collider.enabled = true;
            _collider.transform.SetParent(null);
            _onReleased.OnNext(Unit.Default);

            Vector3 direction = (goalPosition - _collider.transform.position);
            direction.y = 0f;
            direction.Normalize();
            var rad = _throwAngle * Mathf.Deg2Rad;
            direction = new Vector3(
                direction.x * Mathf.Cos(rad),
                Mathf.Sin(rad),
                direction.z * Mathf.Cos(rad)
            );

            var impulse = ThrowForceCalculator.CalculateImpulse(
                    Vector3.Distance(_collider.transform.position, goalPosition),
                    _throwAngle,
                    _bottleRb.mass);

            _bottleRb.isKinematic = false;
            _bottleRb.useGravity = true;
            _bottleRb.AddForce(direction * impulse, ForceMode.Impulse);
            WaitForBottleReleaseAsync(destroyCancellationToken).Forget();
            IsHolding = false;
            return true;
        }

        private async UniTask WaitForBottleReleaseAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(100, cancellationToken: cancellationToken);
            _handCollider.enabled = true;
        }
    }
}