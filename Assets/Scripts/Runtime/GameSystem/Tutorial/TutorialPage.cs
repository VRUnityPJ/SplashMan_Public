using System;
using UnityEngine;

namespace Runtime.GameSystem.Tutorial
{
    /// <summary>
    /// チュートリアルの1ステップで表示する説明（画像と文章）
    /// </summary>
    [Serializable]
    public class TutorialPage
    {
        /// <summary>
        /// 説明用の画像。未設定の場合は画像を表示しない
        /// </summary>
        [SerializeField] private Sprite _image;
        public Sprite Image => _image;

        /// <summary>
        /// 説明文
        /// </summary>
        [SerializeField, TextArea(2, 5)] private string _text;
        public string Text => _text;

        public TutorialPage(string text) => _text = text;
    }
}
