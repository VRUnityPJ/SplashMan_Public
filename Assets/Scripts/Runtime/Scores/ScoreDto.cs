using System;
using UnityEngine;

namespace Runtime.Scores
{
    /// <summary>
    /// スコアの値をシリアライズ可能な形式で表すクラス.
    /// こいつを直接newするのではなく、構造体のScoreを使うこと.
    /// </summary>
    [Serializable]
    public class ScoreDto
    {
        [SerializeField] private int _score;

        /// <summary>
        /// スコアをScore構造体に変換する
        /// </summary>
        /// <returns></returns>
        public Score ToScore() => new(Mathf.Max(0, _score));
    }
}