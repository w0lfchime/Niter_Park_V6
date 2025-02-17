
//----------------------------------------------------------------------------------
// PerformanceSM.cs
//
// Copyright (2025) NITER
//
// This code is part of the PARK-v6 Unity Framework. It is proprietary software.  
// Unauthorized use, modification, or distribution is not permitted without  
// explicit permission from the owner.  
//----------------------------------------------------------------------------------

using System;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public struct SetStateRequest
{
	public Enum StateID;
	public int PushForce;
	public bool ClearOnSetState;

	public SetStateRequest(Enum stateID, int pushForce, bool clearOnStateSwitch)
	{
		StateID = stateID;
		PushForce = pushForce;
		ClearOnSetState = clearOnStateSwitch;
	}
}

public class PerformanceSM
{
	//=//-----|Fields|--------------------------------------------------------//=//

	//Meta 
	public object machineOwner; //e.g. the characters name
	public string machineName;
	public Type stateIDEnum;
	public int stateCount;
	public bool debug = true;

	//State Control
	public Enum currentStateID;
	public PerformanceState currentState;
	protected PerformanceState[] stateArray;
	protected SetStateRequestHeap requestHeap = new();

	//Frames and Time
	public int perFrameProcessLimit = 10;
	private int currentFrame = 0;



	//=//-----|Setup|-----------------------------------------------------------------//=//
	public PerformanceSM(Enum psNullOfStateTypeID, object owner)
	{
		//Some basic Setup
		if (psNullOfStateTypeID.ToString() != "PSNull")
		{
			throw new ArgumentException("Performance State Machines must be created with PSNull enum value of desired enum type. ");
		}
		stateIDEnum = psNullOfStateTypeID.GetType();
		this.currentStateID = psNullOfStateTypeID;
		this.machineOwner = owner;

		stateCount = Enum.GetValues(stateIDEnum).Length;
		stateArray = new PerformanceState[stateCount];

		if (machineOwner is Character character)
		{
			this.machineName = $"{character.characterName} SM";
		}
		else
		{
			this.machineName = $"{machineOwner.GetType().Name} SM";
		}

		RegisterStates();
		VerifyStates();
	}

	//this code is weird as hell. idk why I made this abstract
	protected virtual void RegisterStates()
	{
		if (machineOwner is Character character)
		{
			string stateOwnerClassName = machineOwner.GetType().Name;

			foreach (Enum stateID in Enum.GetValues(stateIDEnum))
			{
				string stateClassName = stateID.ToString();
				if (stateClassName == "PSNull")
				{
					continue;
				}
				if (stateClassName.StartsWith("OO_")) //Owner-Override state 
				{
					stateClassName = stateClassName.Substring(2);

					stateClassName = stateOwnerClassName + stateClassName;
				}
				else
				{
					//good to go 
				}


				Type stateClass = Type.GetType(stateClassName);
				if (stateClass == null)
				{
					LogCore.Log("PSM_Error", $"Failed to generate state from {stateID.ToString()}.");
				}
				else if (stateClass.IsSubclassOf(typeof(CharacterState)))
				{
					CharacterState stateInstance = (CharacterState)Activator.CreateInstance(stateClass, this, character);
					SetStateArrayState(stateID, stateInstance); //put the state in the array
				}
			}
		}

		//other state machines  
		//...
	}

	public virtual void VerifyStates()
	{
		bool passed = true;
		foreach (PerformanceState state in stateArray)
		{
			if (!state.VerifyState())
			{
				LogCore.Log("PSM_Error", $"State {state.stateName} is invlaid.");
				passed = false;
			}
		}
		if (passed)
		{
			LogCore.Log("PSM_Setup", "Successfully verified all registered character states.");
		}
		else
		{
			LogCore.Log("PSM_Error", "Failed to verify all registered character states.");
		}
	}



	//=//-----|State Control|---------------------------------------------------------//=//

	public PerformanceState GetState(Enum stateID)
	{
		int index = Convert.ToInt32(stateID);
		ValidateStateID(stateID); //we dont need this really
		return stateArray[index];
	}
	public PerformanceState GetState()
	{
		int index = Convert.ToInt32(currentStateID);
		return stateArray[index];
	}

	protected void SetStateArrayState(Enum stateID, PerformanceState state)
	{
		int index = Convert.ToInt32(stateID);
		ValidateStateID(stateID);
		stateArray[index] = state;
	}

	protected void ValidateStateID(Enum stateID)
	{
		int index = Convert.ToInt32(stateID);

		if (index <= 0 || index >= stateArray.Length)
		{
			LogCore.Log("PSM_Error", "Tried to validate a out of bounds state.");
		}
	}

	public void PushState(Enum stateID, int pushForce, int lifetime)
	{
		bool coss = GetState(stateID).clearOnSetState;

		var newRequest = new SetStateRequest(stateID, pushForce, coss);

		// Add new state push to the heap (sorted by push force, then LIFO)
		requestHeap.Enqueue(newRequest, lifetime);

		// Immediate override if the new push force is greater than the current state's priority
		if (currentState == null)
		{
			SetCurrentState(stateID);
		}
	}

	public void ForcePushState(Enum stateName)
	{
		//TODO: yeah

	}


	private void ProcessStateHeap()
	{
		if (requestHeap.Count == 0 || currentState.exitAllowed) return;

		// Get the highest-priority state (max push force, LIFO tie-breaker)
		SetStateRequest bestRequest = requestHeap.Peek();

		if (bestRequest.PushForce > currentState.GetPriority())
		{
			SetCurrentState(bestRequest.StateID);
		}
	}

	private void SetCurrentState(Enum newStateID)
	{
		Enum oldStateID = currentStateID;

		currentState.Exit();
		currentState = GetState(newStateID);
		currentStateID = newStateID;

		//state heap
		if (currentState.forceClearStateHeapOnEntry)
		{
			requestHeap.Clear();
		}
		requestHeap.ClearClearOnSetState();

		//good to go
		currentState.Enter();

		LogCore.Log("PSM_Detail", $"Switched from {oldStateID.ToString()} to {currentStateID.ToString()}");
	}


	//Mono
	public void PSMUpdate()
	{
		//...
		currentState.Update();
		//...
	}
	public void PSMFixedFrameUpdate()
	{
		currentFrame++;
		requestHeap.FixedFrameUpdate();
		//...
		currentState.FixedFrameUpdate();
		ProcessStateHeap();
		//...
	}
	public void PSMFixedPhysicsUpdate()
	{
		//...
		currentState.FixedFrameUpdate();
		//...
	}
	public void PSMLateUpdate()
	{
		//...
		currentState.LateUpdate();
		//...
	}
}
