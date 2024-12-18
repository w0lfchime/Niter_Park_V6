using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugCore : MonoBehaviour
{
    public static DebugCore Instance { get; private set; }
    private List<string> logMessages = new List<string>();
    private Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Application.logMessageReceived += HandleUnityLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleUnityLog;
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        Log($"[{type}] {logString}");
    }

    public void Log(string message)
    {
        logMessages.Add(message);
        Debug.Log(message); // Forward to Unity Console
        InGameConsoleUI.Instance?.AddLog(message); // Forward to in-game console if exists
    }

    public void RegisterCommand(string commandName, Action<string[]> action)
    {
        if (!commands.ContainsKey(commandName))
        {
            commands.Add(commandName, action);
        }
        else
        {
            Log($"Command '{commandName}' is already registered.");
        }
    }

    public void ExecuteCommand(string commandInput)
    {
        string[] splitInput = commandInput.Split(' ');
        string commandName = splitInput[0];
        string[] args = splitInput.Length > 1 ? splitInput[1..] : new string[0];

        if (commands.TryGetValue(commandName, out var action))
        {
            action.Invoke(args);
        }
        else
        {
            Log($"Unknown command: {commandName}");
        }
    }

    public IEnumerable<string> GetAllLogs()
    {
        return logMessages;
    }
}
