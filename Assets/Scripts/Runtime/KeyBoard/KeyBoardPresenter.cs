using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Runtime.KeyBoard
{
    /// <summary>
    /// ランキング用の名前入力キーボードのPresenterクラス
    /// このコンポーネント自体は常にアクティブなオブジェクトに置き、表示・非表示は_keyboardRootで切り替える
    /// </summary>
    public class KeyBoardPresenter : MonoBehaviour
    {
        /// <summary>
        /// キーボードUIの親オブジェクト(表示・非表示を切り替える対象)
        /// </summary>
        [SerializeField] private GameObject _keyboardRoot = null!;

        /// <summary>
        /// 入力確定に使う決定ボタン
        /// </summary>
        [SerializeField] private EnterButton _enterButton = null!;

        /// <summary>
        /// キーボードを表示する
        /// 今は即時に切り替えるだけだが、今後表示アニメーションを追加できるようにUniTaskで返している
        /// </summary>
        public UniTask ShowAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _keyboardRoot.SetActive(true);
            // アニメーションを追加する場合はasyncにして、ここでcancellationTokenを渡してawaitする
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// キーボードを非表示にする
        /// 今は即時に切り替えるだけだが、今後非表示アニメーションを追加できるようにUniTaskで返している
        /// </summary>
        public UniTask HideAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _keyboardRoot.SetActive(false);
            // アニメーションを追加する場合はasyncにして、ここでcancellationTokenを渡してawaitする
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 決定ボタンが押されるまで待機
        /// 名前が空のまま決定ボタンが押された場合は無視して待機を続けるのは残す
        /// </summary>
        public async UniTask WaitForNameInputAsync(CancellationToken cancellationToken)
        {
            await _enterButton.OnEntered
                .Where(_ => PlayerNameStorage.GetPlayerName().Length > 0)
                .FirstAsync(cancellationToken: cancellationToken);

        }
    }
}
