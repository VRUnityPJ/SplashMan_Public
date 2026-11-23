using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Runtime.Carbonation.UI
{
    /// <summary>
    /// View: 炭酸ゲージをUIに出力する。IPetBottleViewを実装し、Presenterから駆動される。
    /// ロジックは持たず、表示の中継のみ行う。
    /// Panelにアタッチする想定
    /// </summary>
    public class CarbonationPanelView : MonoBehaviour
    {
        [SerializeField, Tooltip("炭酸ゲージ本体")] private Image _carbonationGauge;
        [SerializeField] private TextMeshProUGUI _carbonationGaugeText;
        [SerializeField] private Canvas _carbonationGaugeCanvas;

        [Header("表示位置")]
        [SerializeField,Tooltip("手のtransformを入れる")]
        private Transform _handTransform;
        [SerializeField,Tooltip("手の位置からどれだけずらして表示するか(例: 頭上に少し浮かせる)")]
        private Vector3 _panelOffset = new(0f, 0.3f, 0f);
        [SerializeField,Tooltip("頭のtransformを入れる")]
        private Transform _headTransform;
        private Vector3 _panelToHead;

        [Header("脈動アニメーション")]
        [SerializeField,Tooltip("脈動させる対象(ゲージのフレームなど)。未設定なら脈動しない")]
        private RectTransform _pulseTarget;
        [SerializeField,Range(0f, 1f),Tooltip("脈動を始めるゲージの割合。ここから100%に近づくほど強く・速くなる")]
        private float _pulseStartRate = 0.7f;
        [SerializeField,Tooltip("ゲージが100%のときの最大の拡大率")]
        private float _pulseMaxScale = 1.15f;
        [SerializeField,Tooltip("脈動を始めた時点での1拍の長さ(秒)")]
        private float _pulseStartInterval = 0.6f;
        [SerializeField,Tooltip("ゲージが100%のときの1拍の長さ(秒)")]
        private float _pulseEndInterval = 0.25f;
        [SerializeField,Tooltip("脈動に合わせてゲージの色を点滅させるか")]
        private bool _enableBlink = true;
        [SerializeField,Tooltip("点滅したときのゲージの色")]
        private Color _blinkColor = Color.white;

        private Vector3 _pulseBaseScale = Vector3.one;
        private Color _gaugeBaseColor = Color.white;
        /// <summary>
        /// 1拍の中での進み具合(0〜1)
        /// </summary>
        private float _pulsePhase;

        private CancellationTokenSource _sliderAnimationCts;

        private void Awake()
        {
            if (_pulseTarget != null)
            {
                _pulseBaseScale = _pulseTarget.localScale;
            }
            if (_carbonationGauge != null)
            {
                _gaugeBaseColor = _carbonationGauge.color;
            }
        }

        private void Update()
        {
            transform.position = _handTransform.position + _panelOffset;
            _panelToHead = transform.position - _headTransform.position;
            transform.rotation = Quaternion.LookRotation(_panelToHead);
            UpdatePulse();
        }

        /// <summary>
        /// ゲージが100%に近づくにつれて、強く・速く脈打たせる
        /// 表示中のゲージ(fillAmount)を基準にするので、ゲージの増減アニメーションと同期する
        /// </summary>
        private void UpdatePulse()
        {
            if (_pulseTarget == null || _carbonationGauge == null)
            {
                return;
            }

            // 脈動の強さ(0〜1)。開始の割合で0、100%で1になる
            var intensity = Mathf.InverseLerp(_pulseStartRate, 1f, _carbonationGauge.fillAmount);
            if (intensity <= 0f)
            {
                _pulsePhase = 0f;
                _pulseTarget.localScale = _pulseBaseScale;
                _carbonationGauge.color = _gaugeBaseColor;
                return;
            }

            var interval = Mathf.Max(0.01f, Mathf.Lerp(_pulseStartInterval, _pulseEndInterval, intensity));
            _pulsePhase = Mathf.Repeat(_pulsePhase + Time.deltaTime / interval, 1f);

            // 拍の前半で素早く膨らみ、後半でゆっくり戻る(ドクンという鼓動の形)
            var beat = Mathf.Sin(Mathf.PI * Mathf.Sqrt(_pulsePhase)) * intensity;

            _pulseTarget.localScale = _pulseBaseScale * Mathf.Lerp(1f, _pulseMaxScale, beat);
            _carbonationGauge.color = _enableBlink
                ? Color.Lerp(_gaugeBaseColor, _blinkColor, beat)
                : _gaugeBaseColor;
        }

        /// <summary>
        ///　炭酸ゲージを見せる
        /// </summary>
        /// <param name="value"></param>
        public void ShowCarbonationGauge(float value)
        {
            if (_carbonationGauge != null)
            {
                if (gameObject.activeInHierarchy)
                {
                    _sliderAnimationCts?.Cancel();
                    _sliderAnimationCts?.Dispose();
                    _sliderAnimationCts = new CancellationTokenSource();
                    AnimateSliderAsync(value, _sliderAnimationCts.Token).Forget();
                    AnimationTextAsync(value, _sliderAnimationCts.Token).Forget();
                }
                else
                {
                    _carbonationGauge.fillAmount = value;
                }
            }
        }

        private async UniTask AnimateSliderAsync(float targetValue, CancellationToken token)
            => await LMotion.Create(_carbonationGauge.fillAmount, targetValue, 0.1f)
                .BindToFillAmount(_carbonationGauge)
                .ToUniTask(cancellationToken: token);

        private async UniTask AnimationTextAsync(float targetValue, CancellationToken token)
            => await LMotion.Create(float.Parse(_carbonationGaugeText.text.Replace("%", "")), targetValue * 100, 0.15f)
                .BindToText(_carbonationGaugeText, "{0:0} %")
                .ToUniTask(cancellationToken: token);

        /// <summary>
        /// 表示/非表示する
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
            => _carbonationGaugeCanvas.enabled = visible;

        public void SetSlider(float maxCarbonation, float currentCarbonation)
        {
            var rate = maxCarbonation > 0 ? currentCarbonation / maxCarbonation : 0;
            if (_carbonationGauge != null)
            {
                _carbonationGauge.fillAmount = rate;
            }
            if (_carbonationGaugeText != null)
            {
                _carbonationGaugeText.text = $"{rate * 100:0} %";
            }
        }
    }
}
