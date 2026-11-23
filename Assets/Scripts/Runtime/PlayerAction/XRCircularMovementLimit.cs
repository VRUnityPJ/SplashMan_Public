using UnityEngine;

namespace Runtime.PlayerAction
{
    public class XRCircularMovementLimit : MonoBehaviour
    {
        [Header("移動範囲")]
        [SerializeField]
        private Vector3 _center;

        [SerializeField]
        private float _radius = 5f;

        private void LateUpdate()
        {
            Vector3 position = transform.position;
            Vector2 offset = new Vector2(
                position.x - _center.x,
                position.z - _center.z
            );
            if (offset.sqrMagnitude > _radius * _radius)
            {
                offset = offset.normalized * _radius;

                position.x = _center.x + offset.x;
                position.z = _center.z + offset.y;

                transform.position = position;
            }
        }
    }
}