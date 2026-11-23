using R3;
using UnityEngine;

namespace Runtime.GameSystem.Timer
{
    public class TimePresenter : MonoBehaviour
    {
        [SerializeField] private GameTimer _gameTimer; // Model
        [SerializeField] private TimeUI _timeUI;       // View

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _gameTimer.CurrentLeftTime.Subscribe(value =>
            {
                _timeUI.SetTimeText(FormatTime(value));
            }).AddTo(this);
        }

        private static string FormatTime(float secondsLeft)
        {
            var totalSeconds = Mathf.Max(0, Mathf.CeilToInt(secondsLeft));
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            return $"残り時間　{minutes:00}:{seconds:00}";
        }
    }
}