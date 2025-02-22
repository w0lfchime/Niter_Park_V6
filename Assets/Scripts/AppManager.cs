using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum PlayableCharacterID
{
    ParkGen2,
    Ric,
    Storm,
}

public enum AppStateID
{

}


public class AppManager : MonoBehaviour //WIP 
{
	//static
	public static AppManager Instance { get; private set; }

	//refs
	private SystemInputManager systemInputManager;

	//appstate
	public AppState currentState;

	//players (local)
	public List<LocalPlayer> localPlayers = new List<LocalPlayer>();

	


	#region mono
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			systemInputManager = new SystemInputManager(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	private void Update()
	{

	}
	private void LateUpdate()
	{

	}
	private void FixedUpdate()
	{

	}
	#endregion mono



	#region setup

	#endregion setup





	#region players_and_input


	public void AddPlayer(InputDevice inputDevice)
	{

	}



	#endregion players_and_input



	#region appstate
	public void SetState(string newState)
    {
		// Exit the current state if it exists
		string oldName = currentState.GetType().Name;

		Type type = Type.GetType(newState);

		if (type == null)
		{
			LogCore.Log("AppManager", $"Failed to generate AppState type from {newState}.");
		}
		else
		{
			currentState?.Exit();
			AppState newAppState = (AppState)Activator.CreateInstance(type);
			currentState = newAppState;
			currentState.Enter();
		}

		LogCore.Log("AppManager", $"Switch from AppState {oldName} to {newState}.");

	}

	#endregion appstate


	#region debug
	void RegisterCommands()
    {
        //CommandHandler.RegisterCommand("appstate", args =>
        //{
        //    switch (args[1])
        //    {
        //        case "set":

        //            break;
        //        case "reset":
        //            AppState newState = (AppState)Activator.CreateInstance(CurrentState.GetType());
        //            SetAppState(newState);
        //            break;
        //        default:
        //            LogCore.Log("CommandMeta", $"Unknown argument: {args[1]}");
        //            break;
        //    }
        //});
    }
	#endregion debug

}
