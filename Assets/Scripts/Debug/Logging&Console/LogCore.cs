using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

public static class LogCore
{
    private static HashSet<string> blacklistedCategories = new HashSet<string>
    {
        //"Software",
        //"Response",
        //"Undefined",
        //"MenuNav",
        //"StateHighDetail",
    };

    public static event Action<string> OnLog;

    public static void Log(string category, string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
    {
        if (!blacklistedCategories.Contains(category))
        {
            string formattedMessage = $"[{category}] {message} (at {System.IO.Path.GetFileName(file)}:{line} in {member})";
            Debug.Log(formattedMessage);
            OnLog?.Invoke(formattedMessage);
        }
    }

    public static void BlacklistCategory(string category)
    {
        blacklistedCategories.Add(category);
    }

    public static void WhitelistCategory(string category)
    {
        blacklistedCategories.Remove(category);
    }

    public static bool IsCategoryBlacklisted(string category)
    {
        return blacklistedCategories.Contains(category);
    }
}
