using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;
    public Button sendButton;
    public Button refreshButton;
    public Button exitButton;
    public ScrollRect consoleScrollRect;
    public Transform consoleContent;
    public TMP_Text consoleMessagePrefab;

    [Header("Settings")]
    public int messageLimit = 100;

    private Queue<GameObject> messageObjects = new Queue<GameObject>();

    private void Start()
    {
        sendButton.onClick.AddListener(SendCommand);
        refreshButton.onClick.AddListener(ClearConsole);
        exitButton.onClick.AddListener(CloseConsole);

        LogCore.OnLog += LogMessage;
    }

    private void 

    private void SendCommand()
    {
        string command = inputField.text;
        if (string.IsNullOrWhiteSpace(command)) return;

        inputField.text = "";
        CommandHandler.ExecuteCommand(command);
    }

    private void LogMessage(string message)
    {
        // Enforce message limit
        if (messageObjects.Count >= messageLimit)
        {
            GameObject oldestMessage = messageObjects.Dequeue();
            Destroy(oldestMessage);
        }

        // Create a new message and add it to the queue
        TMP_Text newMessage = Instantiate(consoleMessagePrefab, consoleContent);
        newMessage.text = message;
        messageObjects.Enqueue(newMessage.gameObject);

        // Force layout and scroll updates
        Canvas.ForceUpdateCanvases();
        consoleScrollRect.verticalNormalizedPosition = 0; // Keep scrolled to the bottom
    }

    private void ClearConsole()
    {
        foreach (Transform child in consoleContent)
        {
            Destroy(child.gameObject);
        }

        messageObjects.Clear();
    }

    private void CloseConsole()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LogCore.OnLog -= LogMessage;
    }
}
