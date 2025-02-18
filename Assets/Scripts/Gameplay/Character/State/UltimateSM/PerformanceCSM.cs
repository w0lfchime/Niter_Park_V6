//----------------------------------------------------------------------------------
// PerformanceCSM.cs
//
// Copyright (2025) NITER
//
// This code is part of the PARK-v6 Unity Framework. It is proprietary software.  
// Unauthorized use, modification, or distribution is not permitted without  
// explicit permission from the owner.  
//----------------------------------------------------------------------------------

using System;
using UnityEngine;

public struct SetStateRequest
{
	public CStateID StateID;
	public int PushForce;
	public bool ClearOnSetState;

	public SetStateRequest(CStateID stateID, int pushForce, bool clearOnStateSwitch)
	{
		StateID = stateID;
		PushForce = pushForce;
		ClearOnSetState = clearOnStateSwitch;
	}

	public override bool Equals(object obj)
	{
		if (obj is SetStateRequest other)
		{
			return StateID == other.StateID &&
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

public class PerformanceCSM
{
	public Character machineOwner;
	public string machineName;
	public int stateCount;
	public bool debug = true;

	public CStateID currentStateID;
	public PerformanceState currentState;
	protected PerformanceState[] stateArray;
	protected RequestQueue requestQueue = new();

	public int perFrameProcessLimit = 10;
	private int currentFrame = 0;

	public PerformanceCSM(Character owner)
	{
		this.currentStateID = CStateID.PSNull;
		this.machineOwner = owner;

		stateCount = Enum.GetValues(typeof(CStateID)).Length;
		stateArray = new PerformanceState[stateCount];

		machineName = $"{machineOwner.characterName} SM";

		RegisterStates();
		VerifyStates();
	}

	protected virtual void RegisterStates()
	{
		string stateOwnerClassName = machineOwner.characterClassName;

		foreach (CStateID stateID in Enum.GetValues(typeof(CStateID)))
		{
			if (stateID == CStateID.PSNull) continue;

			string stateClassName = stateID.ToString();
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
				var stateInstance = (CharacterState)Activator.CreateInstance(stateClass, this, machineOwner);
				SetStateArrayState(stateID, stateInstance);
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

	public PerformanceState GetState(CStateID stateID) => stateArray[(int)stateID];

	public PerformanceState GetState() => stateArray[(int)currentStateID];

	protected void SetStateArrayState(CStateID stateID, PerformanceState state)
	{
		stateArray[(int)stateID] = state;
	}

	public void PushState(CStateID stateID, int pushForce, int frameLifetime)
	{
		bool coss = GetState(stateID).clearOnSetState;
		var newRequest = new SetStateRequest(stateID, pushForce, coss);

		requestQueue.Add(newRequest, frameLifetime);

		if (currentState == null || newRequest.PushForce > currentState.GetPriority())
		{
			SetCurrentState(stateID);
		}
	}

	private void ProcessStateQueue()
	{
		if (requestQueue.Count == 0 || (currentState != null && !currentState.exitAllowed)) return;

		if (requestQueue.TryGetHighestPriority(out SetStateRequest bestRequest))
		{
			if (currentState == null || bestRequest.PushForce > currentState.GetPriority())
			{
				SetCurrentState(bestRequest.StateID);
			}
		}
	}

	private void SetCurrentState(CStateID newStateID)
	{
		CStateID oldStateID = currentStateID;

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
