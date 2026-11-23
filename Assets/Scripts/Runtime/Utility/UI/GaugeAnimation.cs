using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine.UI;

namespace Runtime.Utility.UI;

public static class GaugeAnimation
{
    /// <summary>
    /// ゲージアニメーションを再生する
    /// </summary>
    /// <param name="slider"></param>
    /// <param name="targetValue"></param>
    /// <param name="duration"></param>
    /// <param name="cancellationToken"></param>
    public static async UniTask PlayAsync(
        this Slider slider,
        float targetValue,
        float duration = 0.5f,
        CancellationToken cancellationToken = default)
    {
         await LMotion
            .Create(slider.value, targetValue, duration)
            .Bind(value => slider.value = value)
            .ToUniTask(cancellationToken);
    }
}