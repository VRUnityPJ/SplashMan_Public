using Runtime.Audio;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.GameSystem
{
    /// <summary>
    /// SystemLifetimeScopeのオーディオ関連の依存関係登録をまとめたpartial class。
    /// SeAudioDataBase / BgmAudioDataBaseはインスペクタでアセットを割り当てる(Resourcesは使わない)。
    /// </summary>
    public partial class SystemLifetimeScope
    {
        [SerializeField] private SeAudioDataBase _seAudioDataBase;
        [SerializeField] private BgmAudioDataBase _bgmAudioDataBase;

        /// <summary>
        /// オーディオ関連の依存関係を登録する。Configure()内から呼び出す。
        /// </summary>
        private void ConfigureAudio(IContainerBuilder builder)
        {
            builder.RegisterInstance(_seAudioDataBase);
            builder.RegisterInstance(_bgmAudioDataBase);
            builder.RegisterComponentInHierarchy<AudioSourcePool>();
            builder.RegisterComponentInHierarchy<BgmAudioSource>();
            builder.Register<AudioManager>(Lifetime.Singleton);
        }
    }
}
