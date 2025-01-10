using System;
using UnityEngine;

public static class LogCore
{
    public static event Action<string> OnLog;

    public static void Log(string message)
    {
        Debug.Log(message);
        OnLog?.Invoke(message);
    }

    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
        OnLog?.Invoke($"<color=yellow>{message}</color>");
    }

    public static void LogError(string message)
    {
        Debug.LogError(message);
        OnLog?.Invoke($"<color=red>{message}</color>");
    }
}
