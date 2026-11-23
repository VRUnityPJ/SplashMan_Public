using R3;
using UnityEngine;

namespace Runtime.Bottle
{
    /// <summary>
    /// ボトルが地面に衝突したときの処理を行うクラス
    /// </summary>
    public class BottleCollision : MonoBehaviour
    {
        [SerializeField] private BottleCarbonationStatus _bottleStatus;
        [SerializeField] private Rigidbody _rigidbody;
        private TrailRenderer _bottleTrailRenderer;
        private Collider _collider;
        public bool IsCaught { get; set; } = false;
        private static readonly Subject<(Carbonation.Carbonation, Carbonation.Carbonation ,Vector3)> s_bottleCollisionToAttack = new();
        private static readonly Subject<GameObject> s_bottleCollisionToFactory = new();
        public static Observable<(Carbonation.Carbonation, Carbonation.Carbonation, Vector3)> BottleCollisionToAttack => s_bottleCollisionToAttack;
        public static Observable<GameObject> BottleCollisionToFactory => s_bottleCollisionToFactory;

        private void Awake()
        {
            TryGetComponent(out _collider);
            TryGetComponent(out _bottleTrailRenderer);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground") || other.CompareTag("Enemy"))
            {
                s_bottleCollisionToAttack.OnNext((_bottleStatus.CurrentCarbonation, _bottleStatus.MaxCarbonation, transform.position));
                s_bottleCollisionToFactory.OnNext(gameObject);
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
                _rigidbody.freezeRotation = false;
                _collider.isTrigger = true;
                _bottleTrailRenderer.enabled = false;
                IsCaught = false;
            }
        }
    }
}