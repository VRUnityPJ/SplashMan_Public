using UnityEngine;

namespace Runtime.GameSystem.Ranking;

public static class CalculateResultPosition
{
    private static readonly Vector3 s_fifthPosition = new Vector3(-7.5f, -280f, 0);
    private static readonly Vector3 s_tenthPosition = new Vector3(367.5f, -280f, 0);
    private static readonly float s_yDuration = 120f;

    public static Vector3 GetPosition(int rankingNumber)
    {
        if (rankingNumber <= 5)
        {
            return s_fifthPosition + new Vector3(0, s_yDuration * (5 - rankingNumber), 0);
        }
        else if (rankingNumber <= 10)
        {
            return s_tenthPosition + new Vector3(0, s_yDuration * (10 - rankingNumber), 0);
        }
        return Vector3.zero;
    }
}