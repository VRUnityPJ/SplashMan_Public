using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Utility;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Enemy
{
    /// <summary>
    /// 敵を自動でスポーンさせるクラス。
    /// このコンポーネントが付いた位置が1つの出現地点(スポナー)になる。
    /// ウェーブごとのスポーン設定は自分自身で持ち、EnemyWaveDirectorからウェーブ番号を指定されて動く
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        private const int MillisecondsPerSecond = 1000;

        /// <summary>
        /// ウェーブごとのスポーン設定。インデックスがそのままグローバルウェーブ番号に対応する。
        /// 配列の範囲外、または要素が未設定のウェーブでは休む
        /// </summary>
        [SerializeField] private EnemyWaveSetting[] _waveSettings;

        /// <summary>
        /// スポーンした敵オブジェクトの親（未指定ならこのTransform直下）
        /// </summary>
        [SerializeField] private Transform _enemyParent;

        private IObjectResolver _resolver;

        /// <summary>
        /// スポーンした敵を登録する敵一覧
        /// </summary>
        private EnemyRegistry _enemyRegistry;

        [Inject]
        public void Construct(IObjectResolver resolver, EnemyRegistry enemyRegistry)
        {
            _resolver = resolver;
            _enemyRegistry = enemyRegistry;
        }

        /// <summary>
        /// 指定したウェーブ番号の設定でスポーンし続ける。該当する設定が無ければ何もしない
        /// </summary>
        public async UniTask StartSpawnAsync(int waveIndex, CancellationToken cancellationToken)
        {
            if (_waveSettings == null || waveIndex < 0 || waveIndex >= _waveSettings.Length)
                return;

            var setting = _waveSettings[waveIndex];
            if (setting == null)
                return;

            var intervalMilliseconds = Mathf.RoundToInt(setting.SpawnIntervalSeconds * MillisecondsPerSecond);

            while (!cancellationToken.IsCancellationRequested)
            {
                SpawnBatch(setting);
                await UniTask.Delay(intervalMilliseconds, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// 設定に従い、複数体の敵を同時にスポーンさせる
        /// </summary>
        private void SpawnBatch(EnemyWaveSetting setting)
        {
            for (var i = 0; i < setting.SpawnCountPerInterval; i++)
            {
                SpawnOne(setting);
            }
        }

        /// <summary>
        /// 敵を1体、このスポナーの位置にスポーンさせる
        /// </summary>
        private void SpawnOne(EnemyWaveSetting setting) => Spawn(setting.SelectEnemyType());

        /// <summary>
        /// 指定した種類の敵を1体、このスポナーの位置にスポーンさせる。
        /// スポーンできなかった場合はnullを返す
        /// </summary>
        public EnemyCore Spawn(EnemyMasterData enemyType)
        {
            if (enemyType == null || enemyType.Prefab == null)
            {
                CustomDebug.LogWarning($"{nameof(EnemySpawner)}: スポーンする敵のPrefabが設定されていません ({name})");
                return null;
            }

            var parent = _enemyParent != null ? _enemyParent : transform;

            // 生成と依存注入（[Inject]メソッドの呼び出し）をアトミックに同時に行う
            var enemy = _resolver.Instantiate(enemyType.Prefab, transform.position, Quaternion.identity, parent);

            if (enemy.TryGetComponent<EnemyCore>(out var enemyCore))
            {
                _enemyRegistry.Register(enemyCore);
            }

            return enemyCore;
        }
    }
}