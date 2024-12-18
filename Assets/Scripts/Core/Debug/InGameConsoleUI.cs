using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InGameConsoleUI : MonoBehaviour
{
    public static InGameConsoleUI Instance { get; private set; }
    public GameObject consolePanel;
    public Text logText;
    public InputField inputField;

    private List<string> logBuffer = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        consolePanel.SetActive(false);
        inputField.onEndEdit.AddListener(HandleCommandInput);
    }

    public void AddLog(string message)
    {
        logBuffer.Add(message);
        if (logText != null)
        {
            logText.text = string.Join("\n", logBuffer);
        }
    }

    private void HandleCommandInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        DebugCore.Instance.ExecuteCommand(input);
        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void Update()
    {
        // Toggle console visibility with the backquote key (`)
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            consolePanel.SetActive(!consolePanel.activeSelf);
        }
    }
}
