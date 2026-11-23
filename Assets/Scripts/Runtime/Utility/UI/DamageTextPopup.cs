using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Runtime.Utility.UI
{
    public class DamageTextPopup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textMeshPro;

        [Header("アニメーション設定")]
        [SerializeField] private float _moveUpDistance = 0.8f;
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private Vector3 _pushScale = new Vector3(0.5f, 0.5f, 0.5f);

        public void Show(int damage)
        {
            _textMeshPro.text = damage.ToString();

            Vector3 baseScale = Vector3.one * 0.01f;

            transform.localScale = baseScale * 0.2f;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOScale(baseScale, 0.15f).SetEase(Ease.OutBack));

            sequence.Join(transform.DOMoveY(transform.position.y + _moveUpDistance, _duration).SetEase(Ease.OutSine));

            var popStartTime = _duration - 0.15f;

            sequence.Insert(popStartTime, transform.DOScale(baseScale * 1.3f, 0.06f).SetEase(Ease.OutQuad));

            sequence.Insert(popStartTime + 0.06f, transform.DOScale(Vector3.zero, 0.09f).SetEase(Ease.InBack));
            sequence.Insert(popStartTime + 0.06f, _textMeshPro.DOFade(0f, 0.09f));

            sequence.OnComplete(() =>
            {
                Destroy(gameObject);
            });

        }
    }
}
