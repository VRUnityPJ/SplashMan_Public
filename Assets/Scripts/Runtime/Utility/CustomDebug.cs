using System.Diagnostics;

namespace Runtime.Utility;

/// <summary>
/// UnityEditor上でのみログを出力するためのカスタムデバッグクラス
/// </summary>
public static　class CustomDebug
{
    /// <summary>
    /// UnityEditor上でのみログを出力する。Debug.Logと同様の機能を持つ
    /// </summary>
    /// <param name="o"></param>
    [Conditional("UNITY_EDITOR")]
    public static void Log(object o)
    {
        UnityEngine.Debug.Log(o);
    }
    /// <summary>
    /// UnityEditor上でのみ警告ログを出力する。Debug.LogWarningと同様の機能を持つ
    /// </summary>
    /// <param name="o"></param>
    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(object o)
    {
        UnityEngine.Debug.LogWarning(o);
    }
    /// <summary>
    /// UnityEditor上でのみエラーログを出力する。Debug.LogErrorと同様の機能を持つ
    /// </summary>
    /// <param name="o"></param>
    [Conditional("UNITY_EDITOR")]
    public static void LogError(object o)
    {
        UnityEngine.Debug.LogError(o);
    }
}