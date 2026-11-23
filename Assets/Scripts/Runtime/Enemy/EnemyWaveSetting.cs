using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Enemy
{
    /// <summary>
    /// 1つのスポナーが、あるウェーブで従うスポーン設定（間隔・同時数・敵種類）
    /// </summary>
    [Serializable]
    public class EnemyWaveSetting
    {
        /// <summary>
        /// スポーンの間隔（秒）
        /// </summary>
        [SerializeField] private float _spawnIntervalSeconds = 3f;
        public float SpawnIntervalSeconds => _spawnIntervalSeconds;

        /// <summary>
        /// 1回のスポーンで同時に出現させる敵の数
        /// </summary>
        [SerializeField] private int _spawnCountPerInterval = 1;
        public int SpawnCountPerInterval => _spawnCountPerInterval;

        /// <summary>
        /// trueの場合、_enemyTypesからランダムに敵の種類を選ぶ。falseの場合、_enemyTypesの先頭を使用する
        /// </summary>
        [SerializeField] private bool _useRandomEnemyType = true;
        public bool UseRandomEnemyType => _useRandomEnemyType;

        /// <summary>
        /// スポーン対象となる敵の種類。ランダム時はこの中から抽選される
        /// </summary>
        [SerializeField] private EnemyMasterData[] _enemyTypes;
        public EnemyMasterData[] EnemyTypes => _enemyTypes;

        /// <summary>
        /// 設定に従って敵の種類を選ぶ
        /// </summary>
        public EnemyMasterData SelectEnemyType()
        {
            if (_enemyTypes == null || _enemyTypes.Length == 0)
                return null;

            if (!_useRandomEnemyType)
                return _enemyTypes[0];

            var index = Random.Range(0, _enemyTypes.Length);
            return _enemyTypes[index];
        }
    }
}