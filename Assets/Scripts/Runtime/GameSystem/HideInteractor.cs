using UnityEngine;
using R3;
using Runtime.Utility.UI;

namespace Runtime.GameSystem
{
    /// <summary>
    /// コントローラーから出るインタラクターを見えなくするためのコンポーネント。
    /// </summary>

    public class HideInteractor : MonoBehaviour
    {
        [SerializeField] private GameObject _leftInteractor;
        [SerializeField] private GameObject _rightInteractor;
        [SerializeField] private HoverButton _yesButton;
        [SerializeField] private HoverButton _noButton;

        private void Awake()
        {
            _yesButton.OnDown.Subscribe(_ => Hide());
            _noButton.OnDown.Subscribe(_ => Hide());
        }

        private void Hide()
        {
            _leftInteractor.SetActive(false);
            _rightInteractor.SetActive(false);
        }
    }
}