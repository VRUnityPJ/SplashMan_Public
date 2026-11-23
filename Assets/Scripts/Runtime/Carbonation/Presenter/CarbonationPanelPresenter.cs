using System;
using R3;
using Runtime.Bottle;
using Runtime.Carbonation.UI;
using Runtime.Feedback;
using Runtime.PlayerAction;
using UnityEngine;
using UnityEngine.XR;

namespace Runtime.Carbonation.Presenter
{
    /// <summary>
    /// ViewとModelをつなぐ
    /// Panelにアタッチする想定
    /// </summary>
    public class CarbonationPanelPresenter : MonoBehaviour
    {
        [SerializeField, Tooltip("ゲージがたまる量")] private float _gaugeDelta;

        [Header("Panelに関する紐づけ")]
        [SerializeField] private CarbonationPanelView _view;
        private BottleCarbonationStatus _bottleCarbonationStatus;
        private CalculateCarbonation _model;

        [Header("Playerの状態に関する紐づけ")]
        [SerializeField] private CatchThrowBottle  _catchThrowBottle;
        [SerializeField] private Controller _controller;

        [Header("コントローラーのバイブ")]
        [SerializeField] private XRNode _node;
        [SerializeField, Header("バイブの強さ")] private float _amplitude;
        [SerializeField, Header("バイブの長さ")] private float _duration;

        private IDisposable _shakingSubscription;

        private void Awake()
        {
            _model = new CalculateCarbonation();
            _catchThrowBottle.OnTriggerEnterBottle
                .Subscribe(this,static (other, self) =>
                {
                    self._bottleCarbonationStatus = other.gameObject.GetComponent<BottleCarbonationStatus>();
                    self.SubscribeToShaking();
                    self._view.SetSlider(self._bottleCarbonationStatus.MaxCarbonation.Value, self._bottleCarbonationStatus.CurrentCarbonation.Value);
                    self._view.SetVisible(true);
                }).AddTo(this);
            //TODO:ペットボトルを持った時、投げた時の報告に対しての購読を開始する処理(Panelの表示非表示)
            //↑この中にペットボトルを振っていることの報告に対しての購読を開始と解除を書く
            //PanelVisibleを登録する
            //BottleCarbonationInfoに代入する
            _catchThrowBottle.OnReleased
                .Subscribe(this,static (_, self) =>
                {
                    self._view.SetVisible(false);
                    self.UnsubscribeFromShaking();
                }).AddTo(this);
        }

        private void SubscribeToShaking()
        {
            //TODO:ペットボトルを振っていることの報告に対しての購読を開始する処理
            //CarbonationGaugeを登録する
            _shakingSubscription = _controller.IsShake
                .Where(_ => _controller.IsTriggerPressed && _catchThrowBottle.IsHolding)
                .Subscribe(this, static (_, self) =>
                {
                    var currentCarbonation = self._model.ChangeCarbonation(self._bottleCarbonationStatus, new Carbonation(self._gaugeDelta));
                    var rate = currentCarbonation.Value / self._bottleCarbonationStatus.MaxCarbonation.Value;
                    self._view.ShowCarbonationGauge(rate);
                    VRHaptics.Vibrate(self._node, self._amplitude, self._duration);
                });
        }

        private void UnsubscribeFromShaking()
        {
            //TODO:ペットボトルを振っていることの報告に対しての購読を解除する処理
            _shakingSubscription?.Dispose();
        }

        private void OnDestroy()
        {
            UnsubscribeFromShaking();
        }

        //ペットボトルを持った時、放した時に使用
        [Button]
        private void PanelVisible(bool visible)
            => _view.SetVisible(visible);

        [Button]
        private void SetSlider()
            => _view.SetSlider(_bottleCarbonationStatus.MaxCarbonation.Value, _bottleCarbonationStatus.CurrentCarbonation.Value);

        //ペットボトルを振った時に使用
        [Button]
        private void CarbonationGauge(Carbonation delta)
            => _view.ShowCarbonationGauge(_model.ChangeCarbonation(_bottleCarbonationStatus, delta).Value);
    }
}