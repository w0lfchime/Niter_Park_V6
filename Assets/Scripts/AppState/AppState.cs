using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AppState
{
    public string appState3DScene;
    public string appStateUIScene;

    // Service locator references
    protected UIManager _uiManager { get; private set; }
    protected SettingsManager _settingsManager { get; private set; }

    public AppState()
    {

    }
	//Scenes
	#region scenes
	public void LoadScenes()
    {
		if (appStateUIScene != null)
		{
		    SceneManager.LoadScene(appStateUIScene, LoadSceneMode.Additive);
		}
		if (appState3DScene != null)
		{
			SceneManager.LoadScene(appState3DScene, LoadSceneMode.Additive);
		}
    }
	public void UnloadScenes()
	{
		if (appStateUIScene != null)
		{
			SceneManager.UnloadSceneAsync(appStateUIScene);
		}
		if (appState3DScene != null)
		{
			SceneManager.UnloadSceneAsync(appState3DScene);
		}
	}
	#endregion scenes

	// Lifecycle Methods
	public virtual void Enter()
    {
		LoadScenes();
		//...
    }

    public virtual void Exit()
    {
		//...
		UnloadScenes();
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

}
