using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
	public static AppManager Instance { get; private set; }

	public AppState currentState;
	private string sceneToReload = "Highway"; // The additive scene you want to reload

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	public void ReloadGameplayScene()
	{
		StartCoroutine(ReloadSceneCoroutine());
	}

	private IEnumerator ReloadSceneCoroutine()
	{
		if (SceneManager.GetSceneByName(sceneToReload).isLoaded)
		{
			yield return SceneManager.UnloadSceneAsync(sceneToReload);
		}
		yield return SceneManager.LoadSceneAsync(sceneToReload, LoadSceneMode.Additive);
	}

	public void SetState(string newState)
	{
		string oldName = currentState?.GetType().Name ?? "None";
		Type type = Type.GetType(newState);

		if (type == null)
		{
			Debug.Log($"AppManager: Failed to generate AppState type from {newState}.");
		}
		else
		{
			currentState?.Exit();
			currentState = (AppState)Activator.CreateInstance(type);
			currentState.Enter();
		}

		Debug.Log($"AppManager: Switched from AppState {oldName} to {newState}.");
	}
}
