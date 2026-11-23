using System;
using R3;
using UnityEngine;

namespace Runtime.GameSystem.Timer
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private float _timeLimit = 120.0f; // 制限時間（秒）

        // R3: 外部には読み取り専用（ReadOnlyReactiveProperty）として公開
        private readonly ReactiveProperty<float> _currentLeftTime = new(120.0f);
        public ReadOnlyReactiveProperty<float> CurrentLeftTime => _currentLeftTime;

        // R3: タイムアップ通知用Subject
        private readonly Subject<Unit> _onTimeUpSubject = new();
        public Observable<Unit> OnTimeUp => _onTimeUpSubject;

        private IDisposable _timeDisposable;

        private bool _isTimerRunning;

        /// <summary>
        /// タイマーが動いている(ゲーム中)かどうか
        /// </summary>
        public bool IsRunning => _isTimerRunning;

        public void StartTimer()
        {
            _currentLeftTime.Value = _timeLimit;
            _isTimerRunning = true;

            // 1秒ごとに繰り返す購読を、開始時に一度だけ作る
            _timeDisposable?.Dispose();
            _timeDisposable = Observable.Interval(TimeSpan.FromSeconds(1.0f))
                .Subscribe(_ => Tick())
                .AddTo(this);
        }

        private void Tick()
        {
            if (!_isTimerRunning) return;

            _currentLeftTime.Value -= 1.0f;

            if (_currentLeftTime.Value <= 0.0f)
            {
                TimeUp();
            }
        }

        private void TimeUp()
        {
            StopTimer();
            _onTimeUpSubject.OnNext(Unit.Default);
        }

        public void StopTimer()
        {
            _isTimerRunning = false;
        }

#if UNITY_EDITOR
        /// <summary>
        /// デバッグ用: 制限時間を待たずにタイムアップさせる
        /// </summary>
        [Button("即タイムアップ")]
        private void DebugTimeUp()
        {
            if (!_isTimerRunning) return;

            _currentLeftTime.Value = 0.0f;
            TimeUp();
        }
#endif

        private void OnDestroy()
        {
            _timeDisposable?.Dispose();
            _currentLeftTime.Dispose();
            _onTimeUpSubject.Dispose();
        }
    }
}