using UnityEngine.EventSystems;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Utility.UI;

/// <summary>
/// ボタンの基本的な挙動を提供する抽象クラス
/// これを継承することで、ボタンのクリックやホバーなどのイベントを簡単に扱うことができる
/// 必要に応じて、OnPointerDown、OnPointerUp、OnPointerEnter、OnPointerExitメソッドをオーバーライドしてカスタマイズすることが可能
/// </summary>
public abstract class ButtonBase : Selectable
{
    private readonly Subject<Unit> _onDown = new();
    private readonly Subject<Unit> _onUp = new();
    private readonly Subject<Unit> _onEnter = new();
    private readonly Subject<Unit> _onExit = new();

    /// <summary>
    /// ボタンが押されたときのイベントを購読するためのObservable
    /// </summary>
    public Observable<Unit> OnDown => _onDown;

    /// <summary>
    /// ボタンが離されたときのイベントを購読するためのObservable
    /// </summary>
    public Observable<Unit> OnUp => _onUp;

    /// <summary>
    /// ボタンにカーソルが入ったときのイベントを購読するためのObservable
    /// </summary>
    public Observable<Unit> OnEnter => _onEnter;

    /// <summary>
    /// ボタンからカーソルが出たときのイベントを購読するためのObservable
    /// </summary>
    public Observable<Unit> OnExit => _onExit;

    public override void OnPointerDown(PointerEventData eventData) => _onDown.OnNext(Unit.Default);

    public override void OnPointerUp(PointerEventData eventData) => _onUp.OnNext(Unit.Default);

    public override void OnPointerEnter(PointerEventData eventData) => _onEnter.OnNext(Unit.Default);

    public override void OnPointerExit(PointerEventData eventData) => _onExit.OnNext(Unit.Default);

    protected override void OnDestroy()
    {
        // リソースを解放するために、各SubjectをDisposeする
        base.OnDestroy();
        _onDown.Dispose();
        _onUp.Dispose();
        _onEnter.Dispose();
        _onExit.Dispose();
    }
}