using System;

namespace Runtime.Carbonation
{
    public static class CalculateDamage
    {
        /// <summary>
        /// 炭酸ゲージが0のボトルを当てたときのダメージ(防御力を引く前の値)。
        /// 計算式どおりだと0になってしまうので、固定値を与える
        /// </summary>
        public const int ZeroCarbonationDamage = 10;

        public static int CenterAttackAmount(Carbonation carbonation)
        {
            return (int)(carbonation.Value);
        }

        public static int AreaAttackAmount(int centerDamage, float areaRadius , float distance)
        {
            if (distance < areaRadius/2f)
            {
                return centerDamage * 10;
            }
            //　距離が範囲半径を超えた場合は、範囲半径と同じ値にする
            if (distance > areaRadius)
            {
                distance = areaRadius;
            }
            var normalizedDistance = distance / areaRadius;
            var damageRate = 1f - Math.Pow(normalizedDistance, 2f)/Math.Pow(1.1f, 2f);
            return (int)(centerDamage * damageRate * 10f);
        }
    }
}