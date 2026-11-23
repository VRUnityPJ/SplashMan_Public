using UnityEngine;

namespace Runtime.Audio
{
    /// <summary>
    /// BGM再生専用のAudioSourceを1つ持つコンポーネント。
    /// 常にループ再生を前提とし、切り替え時は即座に前の曲を止めて次の曲を再生する(クロスフェードはしない)。
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class BgmAudioSource : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;
        }

        /// <summary>
        /// 再生中のBGMがあれば即座に停止し、指定したクリップをループ再生する。
        /// </summary>
        public void Play(AudioClip clip)
        {
            _audioSource.Stop();
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        /// <summary>
        /// 再生中のBGMを停止する。
        /// </summary>
        public void Stop()
        {
            _audioSource.Stop();
        }
    }
}
