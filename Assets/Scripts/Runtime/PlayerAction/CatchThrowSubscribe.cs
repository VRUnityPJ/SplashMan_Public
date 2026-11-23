using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Runtime.PlayerAction
{
    public class CatchThrowSubscribe: MonoBehaviour
    {
        [Header("スクリプトの紐づけ")]
        private CatchThrowBottle _catchThrowBottle;
        private Controller _controller;
        private TargetEnemy _targetEnemy;
        private Transform _handTransform;
        private TrailRenderer _bottleTrailRenderer;

        private void Start()
        {
            TryGetComponent(out _catchThrowBottle);
            TryGetComponent(out _controller);
            TryGetComponent(out _targetEnemy);
            TryGetComponent(out _handTransform);
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _catchThrowBottle.OnTriggerEnterBottle
                .Subscribe(this,static (collider, self) =>
                {
                    collider.TryGetComponent(out self._bottleTrailRenderer);
                    self._bottleTrailRenderer.enabled = false;
                    self._catchThrowBottle.CatchBottleAsync(collider, self.destroyCancellationToken).Forget();
                }).AddTo(this);

            _controller.IsThrow
                .Where(_ => !_controller.IsTriggerPressed && _catchThrowBottle.IsHolding)
                .Subscribe(this, static (_, self) =>
                {
                    Debug.Log(self._handTransform.eulerAngles.x);
                    //ボトルを投げた時の処理(振っている時の処理はCarbonationPanelPresenter)
                    Vector3 goalPosition = self._targetEnemy.CurrentTarget != null
                        ? self._targetEnemy.CurrentTarget.position
                        : self._targetEnemy.GetNoTargetingPosition(self._controller.HandSpeed);
                    self._catchThrowBottle.ReleaseAndThrowBottle(goalPosition);
                    self._bottleTrailRenderer.enabled = true;
                }).AddTo(this);
        }
    }
}