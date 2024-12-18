using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [SerializeField] private UIStylesheet stylesheet;

    private Dictionary<string, GameObject> activeElements = new Dictionary<string, GameObject>();


    public Canvas UICanvas;




    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (UICanvas == null)
        {
            // Create a default canvas if not assigned
            GameObject canvasObj = new GameObject("UICanvas");
            UICanvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            UICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    private void OnEnable()
    {
        // Subscribe to the settings change event
        AppManager.Instance.Settings.OnSettingsChanged += UpdateUI();
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        AppManager.Instance.Settings.OnSettingsChanged -= HandleSettingsChanged;
    }

    public void SetCurrentCanvas(Canvas newCanvas)
    {
        if (currentCanvas != null)
        {
            currentCanvas.gameObject.SetActive(false);
        }
        currentCanvas = newCanvas;
        currentCanvas.gameObject.SetActive(true);
    }


    private void HandleScreenSizeChanged(float width, float height)
    {
        Debug.Log($"Screen size changed to: {width}x{height}");
        // Update UI elements dynamically based on new screen size
        UpdateUI(width, height);
    }



    // Element Management - - - - - - - - - - - - - - - - - -
    public void UpdateUI()
    {

    }
    public void ClearAllElements()
    {
        foreach (var element in activeElements.Values)
        {
            Destroy(element);
        }
        activeElements.Clear();
    }

    public void RemoveElement(string name)
    {
        if (activeElements.TryGetValue(name, out GameObject element))
        {
            Destroy(element);
            activeElements.Remove(name);
        }
    }

    //Create panel
    //Create container 

    // Element Creation - - - - - - - - - - - - - - - - - - - - -
    public GameObject CreatePanel(string name, Vector2 size, Vector2 position, Color backgroundColor)
    {

        return null;
    }

    public GameObject CreateText(string name, string content, Vector2 position, int fontSize, TextAnchor alignment)
    {

        return null;
    }

    public GameObject CreateButton(string name, Vector2 size, Vector2 position, string buttonText, Action onClick)
    {


        return null;
    }

    public GameObject CreateContainer(string name, Vector2 size, Vector2 position, bool isVertical = true, int spacing = 10)
    {


        return null;
    }

}
