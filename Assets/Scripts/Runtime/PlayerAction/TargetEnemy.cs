using UnityEngine;

namespace Runtime.PlayerAction
{
    public class TargetEnemy : MonoBehaviour
    {
        /// <summary>
        /// ターゲットをロックオンするクラス
        /// </summary>
        [Header("BoxCastの設定")]
        [SerializeField] private Vector3 _searchBoxSize;
        [SerializeField] private float _maxDistance;
        [SerializeField] private LayerMask _targetLayerMask;

        [Header("ターゲットがいない時の設定")]
        [SerializeField] private Transform _headTransform;

        private bool _isTarget;

        public Transform CurrentTarget { get; private set; }

        private void Update()
        {
            SearchTarget();
        }

        private void SearchTarget()
        {
            var direction = transform.forward.normalized;
            var boxRotation = Quaternion.FromToRotation(Vector3.forward, direction);

            CurrentTarget = Physics.BoxCast(
                transform.position,
                _searchBoxSize,
                direction,
                out var hitInfo,
                boxRotation,
                _maxDistance,
                _targetLayerMask
            ) ? hitInfo.transform : null;
        }

        private void OnDrawGizmosSelected()
        {
            var previousColor = Gizmos.color;
            var previousMatrix = Gizmos.matrix;

            Gizmos.color = Color.green;

            var boxSize = _searchBoxSize * 2f;
            var direction = transform.forward.normalized;
            var boxRotation = Quaternion.FromToRotation(Vector3.forward, direction);
            var castEnd = transform.position + direction * _maxDistance;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, boxRotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            Gizmos.matrix = Matrix4x4.TRS(castEnd, boxRotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            Gizmos.matrix = previousMatrix;
            Gizmos.DrawLine(transform.position, castEnd);

            Gizmos.color = previousColor;
            Gizmos.matrix = previousMatrix;
        }

        /// <summary>
        /// 敵をロックオンしていない場合の投げる位置を返す
        /// </summary>
        /// <param name="handSpeed"></param>
        /// <returns></returns>
        public Vector3 GetNoTargetingPosition(float handSpeed)
        {
            var headForward = _headTransform.forward.normalized;
            var noTargetingPosition = transform.position + new Vector3 (headForward.x, 0, headForward.z) * handSpeed * 1.5f;
            return noTargetingPosition;
        }
    }
}
