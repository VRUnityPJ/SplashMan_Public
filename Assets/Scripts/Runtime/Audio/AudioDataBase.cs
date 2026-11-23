using System;
using System.Collections.Generic;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Audio
{
    /// <summary>
    /// AudioClipを名前で引けるようにまとめるデータベースの基底クラス。
    /// このクラス自体はアセット化せず、SeAudioDataBase / BgmAudioDataBaseなど用途ごとのサブクラスを作って使う
    /// (VContainerに型で登録するため、用途ごとにBGMとSEでを分けている、同じAudioClipDataBaseで登録すると上書きされてしまう？)
    /// </summary>
    public abstract class AudioDataBase : ScriptableObject
    {
        [Serializable]
        private class AudioClipEntry
        {
            [SerializeField] private string _name;
            public string Name => _name;

            [SerializeField] private AudioClip _clip;
            public AudioClip Clip => _clip;
        }

        [SerializeField] private List<AudioClipEntry> _audioClipList = new();

        private Dictionary<string, AudioClip> _audioClipDictionary;

        /// <summary>
        /// 名前が一致するAudioClipを返す。存在しない場合はnullを返す。
        /// </summary>
        public AudioClip GetAudioClip(string name)
        {
            _audioClipDictionary ??= BuildDictionary();

            if (_audioClipDictionary.TryGetValue(name, out var clip))
            {
                return clip;
            }

            CustomDebug.LogWarning($"{GetType().Name}: 名前 \"{name}\" のAudioClipが見つかりません");
            return null;
        }

        private Dictionary<string, AudioClip> BuildDictionary()
        {
            var dictionary = new Dictionary<string, AudioClip>(_audioClipList.Count);
            foreach (var entry in _audioClipList)
            {
                if (string.IsNullOrEmpty(entry.Name))
                {
                    continue;
                }

                if (!dictionary.TryAdd(entry.Name, entry.Clip))
                {
                    CustomDebug.LogWarning($"{GetType().Name}: 名前 \"{entry.Name}\" が重複しています");
                }
            }

            return dictionary;
        }

        // インスペクタでリストを編集した後に古いキャッシュを引きずらないようにする
        private void OnValidate()
        {
            _audioClipDictionary = null;
        }
    }
}
