using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine;

namespace Runtime.Utility.UI
{
    /// <summary>
    /// ホバーアニメーションを行うカスタムボタン
    /// </summary>
    public class HoverButton : ButtonBase
    {
        [SerializeField ,Tooltip("ホバー時の拡大アニメーションの継続時間")]
        private float _duration = 0.2f;

        [SerializeField, Tooltip("ホバー時の拡大率")]
        private float _scaleFactor = 1.01f;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            transform.DOScale(_scaleFactor, _duration);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            transform.DOScale(1f, _duration);
        }
    }
}