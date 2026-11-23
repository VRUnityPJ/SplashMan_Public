using Unity.VisualScripting.Dependencies.NCalc;

namespace Runtime.Audio
{
    /// <summary>
    /// AudioDataBaseに登録した音源名を、文字列リテラルを直書きせず定数経由で参照するための入れ物。
    /// あくまで中身はstringの定数なので、ここに追加した名前と同じ名前をSeAudioDataBase / BgmAudioDataBase側にも登録しておく必要がある。
    /// 呼び出し例: audioManager.PlaySeAsync(AudioType.Player.AttackSound, cancellationToken);
    /// </summary>
    public static class AudioType
    {
        /// <summary>
        /// プレイヤー関連のSE
        /// </summary>
        public static class Player
        {
            public const string AttackSound = "PlayerAttackSound";
            public const string BottleExplosionSound = "BottleExplosionSound";
            public const string BottleEmptyHitSound = "BottleEmptyHitSound";
            public const string BottleOpeningSound = "BottleOpeningSound";
            public const string ShakingSound = "ShakingSound";
            public const string ThrowBottleSound = "ThrowBottleSound";
            public const string CatchBottleSound = "CatchBottleSound";
        }

        /// <summary>
        /// 敵関連のSE
        /// </summary>
        public static class Enemy
        {
            public const string AttackSound = "EnemyAttackSound";
            public const string DefeatedSound = "EnemyDefeatedSE";
            public const string ReachSound = "EnemyReachSound";
        }
        /// <summary>
        /// BGM関係
        /// </summary>
        public static class BGM
        {
            public const string Test = "MasterBGM";
        }

        public static class Result
        {
            public const string TimeUp = "レベルアップ";
            public const string ScoreUp = "スコア上昇";
            public const string ShowBonus = "ボーナス名表示";
        }

        public static class Ranking
        {
            public const string RankingPopup = "RankingPopup";
            public const string RankingPanelPopup = "RankingPanelPopup";
        }
    }
}
