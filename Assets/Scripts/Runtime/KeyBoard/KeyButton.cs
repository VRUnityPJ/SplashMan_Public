using UnityEngine;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;


namespace Runtime.KeyBoard
{
    public class KeyButton : KeyBoardButton
    {
        [SerializeField] private TextUpdater updater;
        [SerializeField] private string myChar;

        private void Start()
        {
            text.text = myChar;
        }

        public override async void Clicked()
        {
            await AddChar();
            updater.UpdateText();
            PlayClickedSe();
        }

        private async UniTask AddChar()
        {
            PlayerNameStorage.AddChar(myChar);
        }
    }
}
