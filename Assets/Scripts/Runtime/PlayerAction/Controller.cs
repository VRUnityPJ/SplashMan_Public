using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.PlayerAction
{
    /// <summary>
    /// コントローラーの入力を受け取るクラス
    /// </summary>
    public class Controller : MonoBehaviour
    {
        [SerializeField] private InputActionReference _inputActRef;
        [SerializeField] private float _shakeThrowSpeed;
        [SerializeField, Min(0.1f)] private float _shakeStopTime;

        [Header("投げられる向きの調整")]
        [SerializeField] Transform _throwDirectionTransform;
        [SerializeField] private Vector2 _throwYawRange = new Vector2(-30f, 30f);
        [SerializeField] private Vector2 _throwPitchRange = new Vector2(-30f, 30f);

        private Vector3 _beforePosition;
        private bool _wasOverShakeThrowSpeed;
        private float _shakeStopTimer;
        private readonly Subject<Unit> _isThrow = new Subject<Unit>();
        private readonly Subject<Unit> _isShake = new Subject<Unit>();
        private readonly ReactiveProperty<bool> _isShaking = new ReactiveProperty<bool>();

        public Observable<Unit> IsThrow => _isThrow;
        public Observable<Unit> IsShake => _isShake;
        public ReadOnlyReactiveProperty<bool> IsShaking => _isShaking;
        public bool IsTriggerPressed { get; private set; }
        public float HandSpeed { get; private set; }

        private void Awake()
        {
            _beforePosition = transform.localPosition;
            _isThrow.AddTo(this);
            _isShake.AddTo(this);
            _isShaking.AddTo(this);
        }

        private void Update()
        {
            UpdateHandStatus();
            FalseIsShaking();
        }

        private void OnEnable()
        {
            _inputActRef.action.started += OnShakingStarted;
            _inputActRef.action.canceled += OnShakingCanceled;
            _inputActRef.action.Enable();
        }

        private void OnDisable()
        {
            _inputActRef.action.started -= OnShakingStarted;
            _inputActRef.action.canceled -= OnShakingCanceled;
            _inputActRef.action.Disable();
        }

        private void OnShakingStarted(InputAction.CallbackContext context)
            => IsTriggerPressed = true;

        private void OnShakingCanceled(InputAction.CallbackContext context)
            => IsTriggerPressed = false;

        private void UpdateHandStatus()
        {
            // ローカル座標じゃないと、急な視点移動やテレポートで速度が大きくなってしまうので、ローカル座標で計算する
            var nowPosition = transform.localPosition;
            var distance = Vector3.Distance(nowPosition, _beforePosition);
            _beforePosition = nowPosition;
            var flameTime = Time.deltaTime;
            if (flameTime <= 0f)
            {
                return;
            }
            HandSpeed = distance / flameTime;
            var isOverShakeThrowSpeed = HandSpeed >= _shakeThrowSpeed;
            if (isOverShakeThrowSpeed && IsThrowableDirection())
            {
                _isThrow.OnNext(Unit.Default);
            }
            if (isOverShakeThrowSpeed && !_wasOverShakeThrowSpeed)
            {
                _isShaking.Value = true;
                _isShake.OnNext(Unit.Default);
            }
            if (isOverShakeThrowSpeed || _wasOverShakeThrowSpeed)
            {
                _shakeStopTimer = 0f;
            }
            else
            {
                _shakeStopTimer += Time.deltaTime;
            }
            _wasOverShakeThrowSpeed = isOverShakeThrowSpeed;
        }

        private void FalseIsShaking()
        {
            if (_shakeStopTimer >= _shakeStopTime)
            {
                _isShaking.Value = false;
            }
        }

        private bool IsThrowableDirection()
        {
            var forward = _throwDirectionTransform.InverseTransformDirection(transform.forward);
            var yaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            var pitch = -Mathf.Asin(Mathf.Clamp(forward.y, -1f, 1f)) * Mathf.Rad2Deg;
            return yaw >= _throwYawRange.x && yaw <= _throwYawRange.y &&
                   pitch >= _throwPitchRange.x && pitch <= _throwPitchRange.y;
        }
    }
}