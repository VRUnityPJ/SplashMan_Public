using UnityEngine;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;

namespace Runtime.KeyBoard
{
    public class DeleteButton : KeyBoardButton
    {
        [SerializeField] private TextUpdater updater;
        public override async void Clicked()
        {
            await DelChar();
            updater.UpdateText();
            PlayClickedSe();
        }

        private async UniTask DelChar()
        {
            PlayerNameStorage.DelChar();
        }
    }
}
