namespace Runtime.GameSystem.Tutorial
{
    /// <summary>
    /// チュートリアルのステップ。上から順に進行する
    /// </summary>
    public enum TutorialStep
    {
        /// <summary>
        /// ボトルをつかむ
        /// </summary>
        Catch,

        /// <summary>
        /// ボトルを振って炭酸ゲージをためる
        /// </summary>
        Carbonation,

        /// <summary>
        /// ボトルを投げて練習用の敵を倒す
        /// </summary>
        Practice,

        /// <summary>
        /// 制限時間とスコアの説明を読んで、ゲームを開始する
        /// </summary>
        Score,
    }
}
