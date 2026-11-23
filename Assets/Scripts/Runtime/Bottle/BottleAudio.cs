using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Runtime.Audio;
using Runtime.PlayerAction;
using UnityEngine;
using VContainer;
using AudioType = Runtime.Audio.AudioType;

namespace Runtime.Bottle
{
    public class BottleAudio : MonoBehaviour
    {
        [SerializeField] private Controller _leftController;
        [SerializeField] private Controller _rightController;
        [SerializeField] private CatchThrowBottle _leftCatchThrowBottle;
        [SerializeField] private CatchThrowBottle _rightCatchThrowBottle;

        private AudioManager _audioManager;
        private CancellationTokenSource _leftShakingCts;
        private CancellationTokenSource _rightShakingCts;

        [Inject]
        private void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void Awake()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            // ボトルが爆発する時の音を再生する購読
            BindAttackSound();

            // ペットボトルをつかんだ時の音を再生する購読を設定する
            BindCatchSound(_leftCatchThrowBottle);
            BindCatchSound(_rightCatchThrowBottle);

            // ペットボトルを投げた時の音を再生する購読を設定する
            BindThrowSound(_leftCatchThrowBottle);
            BindThrowSound(_rightCatchThrowBottle);

            // ペットボトルを振っている時の音を再生する購読を設定する
            BindShakingSound(_leftController, _leftCatchThrowBottle);
            BindShakingSound(_rightController, _rightCatchThrowBottle);
        }

        /// <summary>
        /// ペットボトルをつかんだ時の音を再生する購読を設定する
        /// </summary>
        /// <param name="bottle"></param>
        private void BindCatchSound(CatchThrowBottle bottle)
        {
            bottle.OnTriggerEnterBottle
                .Subscribe(this, static (_, self) =>
                {
                    //ボトルをつかんだ時の処理
                    self._audioManager.PlaySeAsync(AudioType.Player.CatchBottleSound, self.destroyCancellationToken)
                        .Forget();
                }).AddTo(this);
        }

        /// <summary>
        ///  ペットボトルを振っている時の音を再生する購読を設定する
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="bottle"></param>
        private void BindShakingSound(Controller controller, CatchThrowBottle bottle)
        {
            controller.IsShaking
                .Select(isShaking => isShaking && controller.IsTriggerPressed && bottle.IsHolding)
                .DistinctUntilChanged() // 同じ値は無視
                .SubscribeAwait(async (shouldPlay, ct) =>
                {
                    if (!shouldPlay) return; // false が来た時点で、再生中の ct がキャンセルされる
                    await _audioManager.PlaySeLoopAsync(AudioType.Player.ShakingSound, ct);
                }, AwaitOperation.Switch)
                .AddTo(this);
        }

        /// <summary>
        ///  ペットボトルを投げた時の音を再生する購読を設定する
        /// </summary>
        /// <param name="bottle"></param>
        private void BindThrowSound(CatchThrowBottle bottle)
        {
            bottle.OnReleased
                .Subscribe(this, static (_, self) =>
                {
                    //ボトルを投げた時の処理(振っている時の処理はCarbonationPanelPresenter)
                    self._audioManager.PlaySeAsync(AudioType.Player.ThrowBottleSound, self.destroyCancellationToken)
                        .Forget();
                }).AddTo(this);
        }

        /// <summary>
        /// ボトルが爆発する時の音を再生する購読を設定する
        /// </summary>
        private void BindAttackSound()
        {
            BottleCollision.BottleCollisionToAttack
                .Subscribe(this, static (info, self) =>
                {
                    //ボトルが敵に当たった時の処理
                    // 炭酸ゲージが0のボトルは爆発しないので、爆発音の代わりに空のボトルが当たった音を鳴らす
                    var seName = info.Item1.Value > 0f
                        ? AudioType.Player.BottleExplosionSound
                        : AudioType.Player.BottleEmptyHitSound;
                    self._audioManager.PlaySeAsync(seName, self.destroyCancellationToken)
                        .Forget();
                }).AddTo(this);
        }
    }
}