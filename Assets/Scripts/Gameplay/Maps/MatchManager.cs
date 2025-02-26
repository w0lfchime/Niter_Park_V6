using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class MatchManager : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) 
        {
            ReloadGameplayScene();
        }
    }

	private string sceneToReload = "Highway"; // The additive scene you want to reload

	public void ReloadGameplayScene()
	{
		StartCoroutine(ReloadSceneCoroutine());
	}

	private IEnumerator ReloadSceneCoroutine()
	{
		// Unload the gameplay scene
		if (SceneManager.GetSceneByName(sceneToReload).isLoaded)
		{
			yield return SceneManager.UnloadSceneAsync(sceneToReload);
		}

		// Load the gameplay scene additively again
		yield return SceneManager.LoadSceneAsync(sceneToReload, LoadSceneMode.Additive);
	}
}
