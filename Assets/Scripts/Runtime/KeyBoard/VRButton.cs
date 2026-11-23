using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Audio;
using Runtime.GameSystem;
using Runtime.Utility;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.KeyBoard
{
    /// <summary>
    /// キーボードのボタンの基底クラス。
    /// クリック音は既存のAudioManager(SE)経由で再生する。
    /// 鳴らす音はAudioClipではなく、SeAudioDataBaseに登録した名前で指定する。
    /// </summary>
    public class VRButton : MonoBehaviour
    {
        /// <summary>
        /// クリック時に鳴らすSEの名前(SeAudioDataBaseに登録した名前)。空欄なら鳴らさない。
        /// </summary>
        [SerializeField] protected string clickedSeName = KeyBoardAudioType.Click;

        private AudioManager _audioManager;
        private CancellationToken _audioCancellationToken;

        /// <summary>
        /// クリック時のSEを再生する。AudioManagerが取得できない場合は何もしない。
        /// </summary>
        protected void PlayClickedSe()
        {
            if (string.IsNullOrEmpty(clickedSeName))
            {
                return;
            }

            if (!TryResolveAudioManager())
            {
                return;
            }

            _audioManager.PlaySeAsync(clickedSeName, _audioCancellationToken).Forget();
        }

        /// <summary>
        /// SystemLifetimeScopeからAudioManagerを取得してキャッシュする。
        /// キーボードのボタンはVContainerの自動注入対象ではないため、ここでスコープから直接取り出す。
        /// キャンセルトークンはスコープのものを使うので、ボタンが破棄されてもSEは途中で切れない。
        /// </summary>
        private bool TryResolveAudioManager()
        {
            if (_audioManager != null)
            {
                return true;
            }

            var scope = LifetimeScope.Find<SystemLifetimeScope>();
            if (scope == null || scope.Container == null)
            {
                CustomDebug.LogWarning($"{name}: SystemLifetimeScopeが見つからないためクリック音を再生できません");
                return false;
            }

            _audioManager = scope.Container.Resolve<AudioManager>();
            _audioCancellationToken = scope.destroyCancellationToken;
            return true;
        }
    }
}
