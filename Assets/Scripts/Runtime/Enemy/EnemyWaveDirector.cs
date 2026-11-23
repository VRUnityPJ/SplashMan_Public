using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Runtime.GameSystem.Timer;
using Runtime.Utility;
using UnityEngine;
using VContainer;

namespace Runtime.Enemy
{
    /// <summary>
    /// ゲーム全体のウェーブ進行を管理する大本の役割。
    /// ウェーブ番号を先頭から進めながら、登録されている全スポナーに「今のウェーブ番号で動いて」と伝えるだけの管理役。
    /// スポナーごとの間隔・同時数・敵種類はEnemySpawner自身が持っている
    /// </summary>
    public class EnemyWaveDirector : MonoBehaviour
    {
        private const int MillisecondsPerSecond = 1000;

        /// <summary>
        /// このゲームで使う全スポナー
        /// </summary>
        [SerializeField] private EnemySpawner[] _spawners;

        /// <summary>
        /// 各ウェーブの継続時間（秒）。インデックスがそのままウェーブ番号に対応する。
        /// 0以下を指定すると、そのウェーブで無限に継続する（最終ウェーブとして扱う）
        /// </summary>
        [SerializeField] private float[] _waveDurationSeconds;

        /// <summary>
        /// ウェーブ進行全体を止めるためのトークンソース。Stop()でキャンセルする
        /// </summary>
        private CancellationTokenSource _cts;

        /// <summary>
        /// ウェーブが切り替わった時に発火するイベント
        /// </summary>
        public Observable<Unit> OnChangeWave => _onChangeWave;
        private readonly Subject<Unit> _onChangeWave = new();

        /// <summary>
        /// 全ウェーブが終了した時に発火するイベント
        /// </summary>
        public Observable<Unit> OnFinishedAllWaves => _onFinishedAllWaves;
        private readonly Subject<Unit> _onFinishedAllWaves = new();

        /// <summary>
        /// 現在のウェーブ数（1始まり）
        /// </summary>
        public int WaveCount => _waveCount;
        private int _waveCount;

        private GameTimer _gameTimer;

        /// <summary>
        /// 停止時に出現済みの敵を消すための敵一覧
        /// </summary>
        private EnemyRegistry _enemyRegistry;

        [Inject]
        public void Construct(GameTimer gameTimer, EnemyRegistry enemyRegistry)
        {
            _gameTimer = gameTimer;
            _enemyRegistry = enemyRegistry;
        }

        private void Start()
        {
            _gameTimer.OnTimeUp.Subscribe( _ =>
            {
                Stop();
            }).AddTo(this);
        }

        /// <summary>
        /// 外部から呼び出すウェーブ進行の開始メソッド
        /// </summary>
        /// <param name="cancellationToken">ウェーブ進行を中断するためのトークン</param>
        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);
            if (_waveDurationSeconds == null || _waveDurationSeconds.Length == 0)
            {
                CustomDebug.LogWarning($"{nameof(EnemyWaveDirector)}: ウェーブ設定が空のため開始できません");
                return;
            }

            try
            {
                for (var waveIndex = 0; waveIndex < _waveDurationSeconds.Length; waveIndex++)
                {
                    _waveCount = waveIndex + 1;
                    _onChangeWave.OnNext(Unit.Default);

                    await RunWaveAsync(waveIndex, _cts.Token);
                }
            }
            finally
            {
                _onFinishedAllWaves.OnNext(Unit.Default);
            }
        }

        /// <summary>
        /// 1ウェーブ分を実行する。継続時間が0以下の場合は次のウェーブに進まず無限に継続する
        /// </summary>
        private async UniTask RunWaveAsync(int waveIndex, CancellationToken cancellationToken)
        {
            using var waveCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            foreach (var spawner in _spawners)
            {
                spawner.StartSpawnAsync(waveIndex, waveCts.Token).Forget();
            }

            var duration = _waveDurationSeconds[waveIndex];
            if (duration <= 0f)
            {
                await UniTask.WaitUntilCanceled(waveCts.Token);
                return;
            }

            await UniTask.Delay(Mathf.RoundToInt(duration * MillisecondsPerSecond), cancellationToken: waveCts.Token);

            // このウェーブのスポーンを止めて次のウェーブへ進む
            waveCts.Cancel();
        }

        /// <summary>
        /// ウェーブ進行と全スポナーのスポーンを停止し、出現済みの敵も全て消す
        /// </summary>
        public void Stop()
        {
            _cts?.Cancel();
            _enemyRegistry.DespawnAll();
        }

        private void OnDestroy()
        {
            _cts?.Dispose();
            _cts = null;
            _onChangeWave.Dispose();
            _onFinishedAllWaves.Dispose();
        }
    }
}