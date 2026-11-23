using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Management;

namespace Runtime.Utility.UI
{
    /// <summary>
    /// Screen Space - OverlayのCanvasを、VR実行時にWorld Spaceへ切り替えるクラス
    /// ヘッドセットの接続を自動で判定し、接続されていればWorld Space、されていなければOverlayのままにする
    /// ビルドでは常にWorld Spaceになる(本番はVR専用のため)
    /// </summary>
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasModeSwitcher : MonoBehaviour
    {
        /// <summary>
        /// World Space時の頭への追従方法
        /// </summary>
        public enum FollowMode
        {
            /// <summary>
            /// アンカーの位置に固定する
            /// </summary>
            Fixed,

            /// <summary>
            /// 頭の動きに少し遅れてついてくる
            /// </summary>
            Lazy,

            /// <summary>
            /// 頭の動きにすぐついてくる(視界に張り付く)
            /// </summary>
            Immediate,
        }

        /// <summary>
        /// World Space時にCanvasを配置する位置・向き
        /// シーンに空のGameObjectを置いて、シーンビュー上で配置を決める想定
        /// 追従する場合は、起動時のカメラから見たアンカーの位置関係を保ったまま追従する
        /// </summary>
        [SerializeField]
        private Transform _worldAnchor = null!;

        /// <summary>
        /// World Space時のスケール。0.001なら1000pxが1mになる
        /// </summary>
        [SerializeField]
        private float _worldScale = 0.001f;

        /// <summary>
        /// World Space時のCanvasのサイズ(px)
        /// (0, 0)なら、CanvasScalerがScale With Screen Sizeのときは基準解像度、それ以外は1920x1080を使う
        /// </summary>
        [SerializeField]
        private Vector2 _worldCanvasSize = Vector2.zero;

        /// <summary>
        /// アンカー未設定時に、カメラの正面どれくらいの距離に置くか(m)
        /// </summary>
        [SerializeField]
        private float _fallbackDistance = 2.0f;

        /// <summary>
        /// 頭への追従方法。Play中に変更しても反映される
        /// </summary>
        [SerializeField]
        private FollowMode _followMode = FollowMode.Immediate;

        private bool _isWorldSpace;
        private Transform _cameraTransform;
        private Transform _originalParent;
        private LazyFollow _lazyFollow;

        // 起動時に決めた配置(固定時のワールド座標と、追従時のカメラからの相対位置)
        private Pose _fixedPose;
        private Pose _poseFromCamera;

        private void Awake()
        {
            if (!ShouldUseWorldSpace()) return;

            ApplyWorldSpace();
        }

        private static bool ShouldUseWorldSpace()
        {
#if UNITY_EDITOR
            return IsXrLoaderActive();
#else
            return true;
#endif
        }

        /// <summary>
        /// XRの初期化に成功しているかどうか
        /// ヘッドセット未接続のときはOpenXRの初期化に失敗するので、activeLoaderがnullになる
        /// XRDisplaySubsystem.runningはOpenXRのセッション開始(数フレーム後)までfalseのままなので、Awakeの判定には使えない
        /// </summary>
        private static bool IsXrLoaderActive()
        {
            var settings = XRGeneralSettings.Instance;
            var loader = settings != null && settings.Manager != null ? settings.Manager.activeLoader : null;
            CustomDebug.Log($"CanvasModeSwitcher: XR Loader = {(loader != null ? loader.name : "なし")}");
            return loader != null;
        }

        private void ApplyWorldSpace()
        {
            var canvas = GetComponent<Canvas>();
            var mainCamera = Camera.main;

            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = mainCamera;

            // Overlay時の画面と同じサイズにすることで、画面上のレイアウトをそのまま再現する
            var rectTransform = (RectTransform)transform;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = GetCanvasSize();
            rectTransform.localScale = Vector3.one * _worldScale;

            // VRコントローラーでUIを操作するために必要
            if (!TryGetComponent<TrackedDeviceGraphicRaycaster>(out _))
            {
                gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            }

            if (mainCamera == null)
            {
                CustomDebug.LogWarning($"{name}: MainCameraが見つからないので、アンカーの位置に固定します");
                if (_worldAnchor != null) transform.SetPositionAndRotation(_worldAnchor.position, _worldAnchor.rotation);
                return;
            }

            _cameraTransform = mainCamera.transform;
            _originalParent = transform.parent;
            _fixedPose = GetInitialPose();
            _poseFromCamera = new Pose(
                _cameraTransform.InverseTransformPoint(_fixedPose.position),
                Quaternion.Inverse(_cameraTransform.rotation) * _fixedPose.rotation);
            _isWorldSpace = true;

            ApplyFollowMode();
        }

        /// <summary>
        /// 起動時の配置を決める。アンカー未設定ならカメラの正面に置く
        /// </summary>
        private Pose GetInitialPose()
        {
            if (_worldAnchor != null) return new Pose(_worldAnchor.position, _worldAnchor.rotation);

            CustomDebug.LogWarning($"{name}: World Anchorが未設定なので、カメラの正面に配置します");
            var forward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            return new Pose(_cameraTransform.position + forward * _fallbackDistance, Quaternion.LookRotation(forward));
        }

        private void ApplyFollowMode()
        {
            // 一旦すべての追従を解除してから、選択中の方式を適用する
            if (transform.parent != _originalParent) transform.SetParent(_originalParent, true);
            if (_lazyFollow != null) _lazyFollow.enabled = false;

            switch (_followMode)
            {
                case FollowMode.Fixed:
                    transform.SetPositionAndRotation(_fixedPose.position, _fixedPose.rotation);
                    break;

                case FollowMode.Lazy:
                    SetPoseFromCamera();
                    EnableLazyFollow();
                    break;

                case FollowMode.Immediate:
                    // カメラの子にすることで、同じフレームで遅れなく追従させる
                    transform.SetParent(_cameraTransform, false);
                    transform.localPosition = _poseFromCamera.position;
                    transform.localRotation = _poseFromCamera.rotation;
                    break;
            }

            transform.localScale = Vector3.one * _worldScale;
        }

        /// <summary>
        /// 現在のカメラの位置から見て、起動時と同じ位置関係になるように置く
        /// </summary>
        private void SetPoseFromCamera()
            => transform.SetPositionAndRotation(
                _cameraTransform.TransformPoint(_poseFromCamera.position),
                _cameraTransform.rotation * _poseFromCamera.rotation);

        /// <summary>
        /// XR Interaction ToolkitのLazyFollowで、カメラに少し遅れて追従させる
        /// Overlay時に動かないよう、VR時だけ実行時に追加する
        /// </summary>
        private void EnableLazyFollow()
        {
            if (_lazyFollow == null)
            {
                _lazyFollow = gameObject.AddComponent<LazyFollow>();
                _lazyFollow.positionFollowMode = LazyFollow.PositionFollowMode.Follow;
                _lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.LookAtWithWorldUp;
                // AddComponent直後のOnEnableはオフセット設定前に走っているので、一旦無効にしておく
                _lazyFollow.enabled = false;
            }

            _lazyFollow.target = _cameraTransform;
            _lazyFollow.targetOffset = _cameraTransform.InverseTransformVector(transform.position - _cameraTransform.position);
            _lazyFollow.enabled = true;
        }

        private Vector2 GetCanvasSize()
        {
            if (_worldCanvasSize != Vector2.zero) return _worldCanvasSize;

            return TryGetComponent<CanvasScaler>(out var scaler) &&
                   scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize
                ? scaler.referenceResolution
                : new Vector2(1920f, 1080f);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Play中にInspectorで追従方法を変えたら、すぐに反映する
        /// </summary>
        private void OnValidate()
        {
            if (!Application.isPlaying || !_isWorldSpace) return;

            ApplyFollowMode();
        }

        /// <summary>
        /// VR時にCanvasが表示される範囲をシーンビューに表示する
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_worldAnchor == null) return;

            var size = GetCanvasSize();
            Gizmos.color = Color.cyan;
            Gizmos.matrix = Matrix4x4.TRS(_worldAnchor.position, _worldAnchor.rotation, Vector3.one * _worldScale);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 0f));
            Gizmos.matrix = Matrix4x4.identity;
        }
#endif
    }
}
