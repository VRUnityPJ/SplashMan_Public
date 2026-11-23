using System.Collections.Generic;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Audio
{
    /// <summary>
    /// SE再生用のAudioSourceをあらかじめ複数用意しておき、使い回すためのプール。
    /// SEの同時再生数は基本的にプールサイズが上限になる(枯渇時は追加生成して警告を出す)。
    /// </summary>
    public class AudioSourcePool : MonoBehaviour
    {
        [SerializeField] private int _poolSize = 8;

        private readonly Queue<AudioSource> _idleSources = new();

        private void Awake()
        {
            for (var i = 0; i < _poolSize; i++)
            {
                _idleSources.Enqueue(CreateSource());
            }
        }

        /// <summary>
        /// 空いているAudioSourceを1つ借りる。プールが枯渇している場合は新規に追加生成する。
        /// </summary>
        public AudioSource Rent()
        {
            if (_idleSources.Count > 0)
            {
                return _idleSources.Dequeue();
            }

            CustomDebug.LogWarning($"{nameof(AudioSourcePool)}: プールが枯渇したためAudioSourceを追加生成します");
            return CreateSource();
        }

        /// <summary>
        /// 使い終わったAudioSourceをプールに返却する。
        /// </summary>
        public void Return(AudioSource source)
        {
            source.clip = null;
            _idleSources.Enqueue(source);
        }

        private AudioSource CreateSource()
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }
    }
}
