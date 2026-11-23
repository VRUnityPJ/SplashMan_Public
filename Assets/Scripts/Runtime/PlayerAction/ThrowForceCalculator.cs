using UnityEngine;

namespace Runtime.PlayerAction
{
    public static class ThrowForceCalculator
    {
        /// <summary>
        /// 指定した距離を、指定した角度で飛ばすために必要な
        /// Impulse（瞬間的に与える力積）を計算する。
        /// AI生成
        /// </summary>
        /// <param name="distance">飛ばしたい距離</param>
        /// <param name="angle">投げる角度（度）</param>
        /// <param name="mass">RigidbodyのMass</param>
        /// <returns>AddForceに渡すImpulseの大きさ</returns>
        public static float CalculateImpulse(
            float distance,
            float angle,
            float mass)
        {
            var gravity = Mathf.Abs(Physics.gravity.y);
            var angleRad = angle * Mathf.Deg2Rad;
            var velocity = Mathf.Sqrt(
                distance * gravity /
                Mathf.Sin(2f * angleRad)
            );
            return mass * velocity;
        }
    }
}