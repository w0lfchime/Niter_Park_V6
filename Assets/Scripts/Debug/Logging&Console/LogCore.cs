using System;
using System.Collections.Generic;
using UnityEngine;

public static class LogCore
{

    private static HashSet<LogCategory> enabledCategories = new HashSet<LogCategory>
    {
        LogCategory.Software,
        LogCategory.Response,
        LogCategory.Undefined, // Default enabled categories
        LogCategory.MenuNav,
    };


    public static event Action<string> OnLog;

    public static void Log(string message, LogCategory category = LogCategory.Undefined)
    {
        if (enabledCategories.Contains(category))
        {
            string formattedMessage = $"[{category}] {message}";
            Debug.Log(formattedMessage);
            OnLog?.Invoke(formattedMessage);
        }
    }

    public static void LogError(string message, LogCategory category = LogCategory.Undefined)
    {
        if (enabledCategories.Contains(category))
        {
            string formattedMessage = $"<color=red>[{category}] {message}</color>";
            Debug.LogError(formattedMessage);
            OnLog?.Invoke(formattedMessage);
        }
    }

    public static void EnableCategory(LogCategory category)
    {
        enabledCategories.Add(category);
    }

    public static void DisableCategory(LogCategory category)
    {
        enabledCategories.Remove(category);
    }

    public static void ToggleCategory(LogCategory category)
    {
        if (enabledCategories.Contains(category))
            enabledCategories.Remove(category);
        else
            enabledCategories.Add(category);
    }

    public static bool IsCategoryEnabled(LogCategory category)
    {
        return enabledCategories.Contains(category);
    }
}
