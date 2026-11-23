using Runtime.Bottle;
using UnityEngine;

namespace Runtime.Carbonation
{
    /// <summary>
    /// 炭酸ゲージの計算処理
    /// </summary>
    public class CalculateCarbonation
    {
        /// <summary>
        /// 炭酸ゲージを変化させる（0〜最大値でクランプ）。
        /// deltaが正なら増加、負なら減少。増減どちらにするかは呼び出し側が決める。
        /// </summary>
        public Carbonation ChangeCarbonation(BottleCarbonationStatus bottleStatus, Carbonation delta)
        {
            if (delta == new Carbonation(0))
            {
                return bottleStatus.CurrentCarbonation;
            }

            var clamped
                = Mathf.Clamp(
                    bottleStatus.CurrentCarbonation.Value + delta.Value,
                    bottleStatus.MinCarbonation.Value,
                    bottleStatus.MaxCarbonation.Value);
            bottleStatus.SetCurrentCarbonation(new Carbonation(clamped));
            return bottleStatus.CurrentCarbonation;
        }
    }
}