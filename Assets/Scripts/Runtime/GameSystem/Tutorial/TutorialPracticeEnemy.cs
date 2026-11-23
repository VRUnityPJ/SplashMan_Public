using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Runtime.Enemy;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.GameSystem.Tutorial
{
    /// <summary>
    /// チュートリアルの練習用の敵を管理するクラス
    /// </summary>
    public class TutorialPracticeEnemy : MonoBehaviour
    {
        /// <summary>
        /// 練習用の敵を出現させるスポナー
        /// </summary>
        [SerializeField] private EnemySpawner _spawner = null!;

        /// <summary>
        /// 練習用に出現させる敵の種類
        /// </summary>
        [SerializeField] private EnemyMasterData _enemyType = null!;

        /// <summary>
        /// 倒せずにプレイヤーへ到達された後、出し直すまでの待ち時間（秒）
        /// </summary>
        [SerializeField] private float _respawnDelaySeconds = 1f;

        /// <summary>
        /// 練習用の敵を1体出現させ、倒されるまで待機する。
        /// 倒される前にプレイヤーへ到達して消えた場合は、同じ敵を出し直す
        /// </summary>
        public async UniTask WaitForDefeatAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var enemy = _spawner != null ? _spawner.Spawn(_enemyType) : null;
                if (enemy == null)
                {
                    CustomDebug.LogWarning($"{nameof(TutorialPracticeEnemy)}: 練習用の敵をスポーンできないため、練習をスキップします");
                    return;
                }

                var isDefeated = false;
                using (enemy.OnDefeated.Subscribe(_ => isDefeated = true))
                {
                    // 到達して消えた場合は敵が破棄されるので、撃破とあわせて監視する
                    await UniTask.WaitUntil(() => isDefeated || enemy == null, cancellationToken: cancellationToken);
                }

                if (isDefeated) return;

                await UniTask.Delay(TimeSpan.FromSeconds(_respawnDelaySeconds), cancellationToken: cancellationToken);
            }
        }
    }
}
