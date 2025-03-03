using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class AppManager : MonoBehaviour
{
	public static AppManager Instance { get; private set; }

	public AppState currentState;
	private string sceneToReload = "Highway"; // The additive scene you want to reload

	private float lastInputTime;
	private const float inactivityThreshold = 20f; // Time in seconds before reload
	private bool moved = false;

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

	private void Start()
	{
		lastInputTime = Time.time;

		// Register input event for ANY button press (Keyboard, Mouse, or Controller)
		InputSystem.onAnyButtonPress.Call(_ => ResetInactivityTimer());
	}

	private void Update()
	{
		ProcessInput();
		CheckControllerActivity(); // Check for analog stick movement
		if (!moved)
		{
			lastInputTime = Time.time;
		}

		// If inactivity threshold is exceeded, reload the scene
		if (Time.time - lastInputTime > inactivityThreshold)
		{
			ReloadGameplayScene();
			lastInputTime = Time.time; // Reset timer to avoid multiple reloads
		}
	}

	private void ProcessInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0)) // Manual Reload with Key 0
		{
			ReloadGameplayScene();
		}
	}

	private void CheckControllerActivity()
	{
		foreach (var gamepad in Gamepad.all) // Loop through all connected controllers
		{
			if (gamepad == null) continue;

			// Detect if ANY stick has moved past a deadzone (prevent accidental drift triggering)
			if (gamepad.leftStick.ReadValue().magnitude > 0.1f ||
				gamepad.rightStick.ReadValue().magnitude > 0.1f ||
				gamepad.dpad.ReadValue().magnitude > 0.1f)
			{
				ResetInactivityTimer();
				moved = true;
			}
		}
	}

	private void ResetInactivityTimer()
	{
		lastInputTime = Time.time;
	}

	public void ReloadGameplayScene()
	{
		moved = false;
		VectorRenderManager vrm = GetComponent<VectorRenderManager>();
		vrm.ResetVectors();
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
