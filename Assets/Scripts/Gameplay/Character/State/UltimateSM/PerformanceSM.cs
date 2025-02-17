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
using System.Linq;
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

	public override bool Equals(object obj)
	{
		if (obj is SetStateRequest other)
		{
			return StateID.Equals(other.StateID) &&
				   PushForce == other.PushForce &&
				   ClearOnSetState == other.ClearOnSetState;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(StateID, PushForce, ClearOnSetState);
	}
}

public class PerformanceSM
{
	//=//-----|Fields|--------------------------------------------------------//=//

	//Meta 
	public object machineOwner;
	public string machineName;
	public Type stateIDEnum;
	public int stateCount;
	public bool debug = true;

	//State Control
	public Enum currentStateID;
	public PerformanceState currentState;
	protected PerformanceState[] stateArray;
	protected RequestQueue requestQueue = new();

	//Frames and Time
	public int perFrameProcessLimit = 10;
	private int currentFrame = 0;

	//=//-----|Setup|-----------------------------------------------------------------//=//
	public PerformanceSM(Enum psNullOfStateTypeID, object owner)
	{
		if (psNullOfStateTypeID.ToString() != "PSNull")
		{
			throw new ArgumentException("Performance State Machines must be created with PSNull enum value of desired enum type.");
		}

		stateIDEnum = psNullOfStateTypeID.GetType();
		this.currentStateID = psNullOfStateTypeID;
		this.machineOwner = owner;

		stateCount = Enum.GetValues(stateIDEnum).Length;
		stateArray = new PerformanceState[stateCount];

		machineName = (machineOwner is Character character)
			? $"{character.characterName} SM"
			: $"{machineOwner.GetType().Name} SM";

		RegisterStates();
		VerifyStates();
	}

	protected virtual void RegisterStates()
	{
		if (machineOwner is Character character)
		{
			string stateOwnerClassName = machineOwner.GetType().Name;

			foreach (Enum stateID in Enum.GetValues(stateIDEnum))
			{
				string stateClassName = stateID.ToString();
				if (stateClassName == "PSNull") continue;

				if (stateClassName.StartsWith("OO_"))
				{
					stateClassName = stateOwnerClassName + stateClassName.Substring(2);
				}

				Type stateClass = Type.GetType(stateClassName);
				if (stateClass == null)
				{
					LogCore.Log("PSM_Error", $"Failed to generate state from {stateID}.");
					continue;
				}

				if (stateClass.IsSubclassOf(typeof(CharacterState)))
				{
					var stateInstance = (CharacterState)Activator.CreateInstance(stateClass, this, character);
					SetStateArrayState(stateID, stateInstance);
				}
			}
		}
	}

	public virtual void VerifyStates()
	{
		bool passed = true;
		foreach (PerformanceState state in stateArray)
		{
			if (!state.VerifyState())
			{
				LogCore.Log("PSM_Error", $"State {state.stateName} is invalid.");
				passed = false;
			}
		}

		LogCore.Log(passed ? "PSM_Setup" : "PSM_Error",
			passed ? "Successfully verified all registered character states."
				   : "Failed to verify all registered character states.");
	}

	//=//-----|State Control|---------------------------------------------------------//=//

	public PerformanceState GetState(Enum stateID) => stateArray[Convert.ToInt32(stateID)];

	public PerformanceState GetState() => stateArray[Convert.ToInt32(currentStateID)];

	protected void SetStateArrayState(Enum stateID, PerformanceState state)
	{
		stateArray[Convert.ToInt32(stateID)] = state;
	}

	public void PushState(Enum stateID, int pushForce, int frameLifetime)
	{
		//HACK: LOG DETAIL EXTREME for debug 
		bool coss = GetState(stateID).clearOnSetState;
		var newRequest = new SetStateRequest(stateID, pushForce, coss);

		// Add new request or update lifetime if it already exists
		requestQueue.Add(newRequest, frameLifetime);

		// Immediate override if the new push force is greater than the current state's priority
		if (currentState == null || newRequest.PushForce > currentState.GetPriority())
		{
			SetCurrentState(stateID);
		}
	}

	private void ProcessStateQueue()
	{
		if (requestQueue.Count == 0 || (currentState != null && !currentState.exitAllowed)) return;

		// Get the highest-priority state
		if (requestQueue.TryGetHighestPriority(out SetStateRequest bestRequest))
		{
			if (currentState == null || bestRequest.PushForce > currentState.GetPriority())
			{
				SetCurrentState(bestRequest.StateID);
			}
		}
	}

	private void SetCurrentState(Enum newStateID)
	{
		Enum oldStateID = currentStateID;

		currentState?.Exit();
		currentState = GetState(newStateID);
		currentStateID = newStateID;

		if (currentState.forceClearStateHeapOnEntry)
		{
			requestQueue.Clear();
		}

		requestQueue.ClearClearOnSetState();
		currentState.Enter();

		LogCore.Log("PSM_Detail", $"Switched from {oldStateID} to {currentStateID}");
	}

	//Mono
	public void PSMUpdate()
	{
		currentState?.Update();
	}

	public void PSMFixedFrameUpdate()
	{
		currentFrame++;

		requestQueue.FixedFrameUpdate();
		currentState?.FixedFrameUpdate();
		ProcessStateQueue();
	}

	public void PSMFixedPhysicsUpdate()
	{
		currentState?.FixedPhysicsUpdate();
	}

	public void PSMLateUpdate()
	{
		currentState?.LateUpdate();
	}
}
