using UnityEngine;
using UnityEngine.XR;

namespace Runtime.Feedback
{
    /// <summary>
    /// 指定したコントローラーに何かしらの振動によるフィードバックを送るクラス
    /// </summary>
    public static class VRHaptics
    {
        /// <summary>
        /// 指定したコントローラーにバイブレーションを送る
        /// </summary>
        /// <param name="node">XRNode.LeftHand または XRNode.RightHand</param>
        /// <param name="amplitude">強さ（0.0 ～ 1.0）</param>
        /// <param name="duration">長さ（秒数）</param>
        public static void Vibrate(XRNode node, float amplitude = 0.5f, float duration = 0.1f)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(node);

            if (device.isValid && device.TryGetHapticCapabilities(out var capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    device.SendHapticImpulse(0u, amplitude, duration);
                }
            }
        }
    }
}