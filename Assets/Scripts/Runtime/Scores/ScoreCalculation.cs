namespace Runtime.Scores
{
    /// <summary>
    /// スコア計算を行うクラス.
    /// </summary>
    public static class ScoreCalculation
    {
        /// <summary>
        /// スコア計算を行うメソッド.
        /// 一旦は現在のスコアをそのまま返すだけの実装.
        /// </summary>
        /// <param name="currentScore"></param>
        /// <returns></returns>
        public static Scores.Score CalculateScore(Score currentScore)
        {
            return currentScore;
        }
    }
}