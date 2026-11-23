using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Editor
{
    /// <summary>
    /// AudioClipまとめるデータベース。
    /// </summary>
    [CreateAssetMenu(menuName = "Condition_Scriptable")]
    public class AudioDataBase : ScriptableObject
    {
        public List<AudioClip>  AudioClipList = new List<AudioClip>();

        /// <summary>
        /// id番目のAudioClipを返す。
        /// </summary>
        /// <param name="id"></param>
        public AudioClip GetAudioClip(int id)
        {
            return null;
        }
        /// <summary>
        /// 名前が一致するAudioClipを返す。存在しない場合nullを返す。
        /// </summary>
        /// <param name="id"></param>
        public AudioClip GetAudioClip(string name)
        {
            return null;
        }
    }
}