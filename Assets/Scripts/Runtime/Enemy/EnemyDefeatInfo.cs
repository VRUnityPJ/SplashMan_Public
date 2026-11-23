using Runtime.Scores;
using UnityEngine;

namespace Runtime.Enemy
{
    /// <summary>
    /// 敵が撃破されたときに通知する情報.
    /// EnemyMasterDataをそのまま流すと、撃破とは関係ない項目(Hp/Attack/MoveSpeedなど)まで
    /// 購読側に渡ってしまうため、撃破イベント用のデータとして切り出したもの.
    /// </summary>
    public readonly struct EnemyDefeatInfo
    {
        public EnemyMasterData MasterData { get; }

        public Score Score => new Score(MasterData.Score);

        public Vector3 DeadPosition { get; }

        public EnemyDefeatInfo(EnemyMasterData masterData, Vector3 deadPosition)
        {
            MasterData = masterData;
            DeadPosition = deadPosition;
        }
    }
}
