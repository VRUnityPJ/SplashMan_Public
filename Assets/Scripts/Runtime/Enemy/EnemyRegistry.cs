using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Runtime.Enemy
{
    /// <summary>
    /// 現在シーンに存在する敵の一覧を管理するクラス。
    /// 敵はスポナーが生成時に登録し、破棄されると自動で一覧から外れる。
    /// 今いる敵と撃破した敵の一覧は、インスペクターで確認できる
    /// </summary>
    public class EnemyRegistry : MonoBehaviour
    {
        /// <summary>
        /// 現在存在する敵（確認用にインスペクターへ表示している）
        /// </summary>
        [SerializeField] private List<EnemyCore> _enemies = new();

        /// <summary>
        /// 撃破された敵のデータ（撃破順。確認用にインスペクターへ表示している）。
        /// Despawnで消えた敵は含まない
        /// </summary>
        [SerializeField] private List<EnemyMasterData> _defeatedEnemies = new();

        /// <summary>
        /// 敵を一覧に登録する。敵が破棄されたら（撃破・Despawnどちらでも）自動で一覧から外す
        /// </summary>
        public void Register(EnemyCore enemy)
        {
            _enemies.Add(enemy);
            enemy.GetCancellationTokenOnDestroy().Register(() => _enemies.Remove(enemy));

            // 撃破されたときだけ撃破一覧に残す（DespawnではOnDefeatedが発火しないので入らない）
            enemy.OnDefeated
                .Subscribe(info => _defeatedEnemies.Add(info.MasterData))
                .AddTo(enemy);
        }

        /// <summary>
        /// 一覧にある敵を、撃破扱いにせず全て消す
        /// </summary>
        public void DespawnAll()
        {
            // Despawn中に一覧が変更されても影響しないよう、コピーしてから回す
            foreach (var enemy in _enemies.ToArray())
            {
                enemy.Despawn();
            }

            _enemies.Clear();
        }
    }
}
