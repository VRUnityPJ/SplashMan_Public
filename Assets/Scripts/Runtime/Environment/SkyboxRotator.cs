using Runtime.Utility;
using UnityEngine;

namespace Runtime.Environment
{
    /// <summary>
    /// Skyboxをゆっくり回転させて、雲が流れているように見せるクラス
    /// Skybox/CubemapやSkybox/Panoramicなど、_Rotationプロパティを持つSkyboxに対応
    /// </summary>
    public class SkyboxRotator : MonoBehaviour
    {
        private static readonly int s_rotationId = Shader.PropertyToID("_Rotation");

        /// <summary>
        /// 1秒あたりの回転角度(度)。マイナスで逆回転
        /// VRで速くしすぎると酔いやすいので、1〜2程度がおすすめ
        /// </summary>
        [SerializeField] private float _degreesPerSecond = 1.0f;

        private Material _originalSkybox;
        private Material _skyboxInstance;
        private float _rotation;

        private void Awake()
        {
            _originalSkybox = RenderSettings.skybox;
            if (_originalSkybox == null || !_originalSkybox.HasProperty(s_rotationId))
            {
                CustomDebug.LogWarning("SkyboxRotator: Skyboxが未設定か、_Rotationプロパティを持たないSkyboxなので回転できません");
                enabled = false;
                return;
            }

            // マテリアルアセットを直接書き換えると、エディタで再生を止めても回転が残ってしまうので複製して使う
            _skyboxInstance = new Material(_originalSkybox);
            RenderSettings.skybox = _skyboxInstance;
            _rotation = _skyboxInstance.GetFloat(s_rotationId);
        }

        private void Update()
        {
            _rotation = Mathf.Repeat(_rotation + _degreesPerSecond * Time.deltaTime, 360f);
            _skyboxInstance.SetFloat(s_rotationId, _rotation);
        }

        private void OnDestroy()
        {
            if (_skyboxInstance == null) return;

            if (RenderSettings.skybox == _skyboxInstance)
            {
                RenderSettings.skybox = _originalSkybox;
            }
            Destroy(_skyboxInstance);
        }
    }
}
