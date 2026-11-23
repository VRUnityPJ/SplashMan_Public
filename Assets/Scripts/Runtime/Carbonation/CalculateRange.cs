using Unity.Mathematics.Geometry;
using UnityEngine;

namespace Runtime.Carbonation;

public static class CalculateRange
{
    // 炭酸の値を100%としたときの最大範囲
    // TODO : 爆発範囲を可視化できるようになったら調整
    private const float OneHundredRange = 8f;

    public static float Calculate(Carbonation carbonation)
    {
        var normalized = carbonation.Value / 100f;
        var value = OneHundredRange * Mathf.Pow(normalized, 1f / 3f);
        return Mathf.Max(1f, value);
    }
}