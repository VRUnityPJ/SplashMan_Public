using R3;

namespace Runtime.KeyBoard
{
    /// <summary>
    /// 名前入力を確定する決定ボタン
    /// 押されたことはOnEnteredで購読できる
    /// </summary>
    public class EnterButton : KeyBoardButton
    {
        private readonly Subject<Unit> _onEntered = new();

        /// <summary>
        /// 決定ボタンが押されたときのイベントを購読するためのObservable
        /// </summary>
        public Observable<Unit> OnEntered => _onEntered;

        public override void Clicked()
        {
            PlayClickedSe();
            _onEntered.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            _onEntered.Dispose();
        }
    }
}
