
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

public class PerformanceSM
{
	//=//-----|Fields|--------------------------------------------------------//=//

	//Meta 
	public object owner; //e.g. the characters name
	public Type stateIDEnum;
	public int stateCount;
	public bool debug = true;

	//State Control
	public Enum currentStateID;
	public PerformanceState currentState;
	protected PerformanceState[] stateArray;
	protected SetStateRequestHeap<SetStateRequest> requestHeap = new();

	//Frames and Time
	public int perFrameProcessLimit = 10;
	private int currentFrame = 0;

	protected struct SetStateRequest
	{
		public Enum StateID;
		public int PushFrame; // Used for LIFO sorting
		public int PushLifetime;
		public int PushForce;
		public bool ClearOnStateSwitch;

		public SetStateRequest(Enum stateID, int pushForce, int expirationFrame, bool clearOnStateSwitch, int pushFrame)
		{
			StateID = stateID;
			PushForce = pushForce;
			PushLifetime = expirationFrame;
			ClearOnStateSwitch = clearOnStateSwitch;
			PushFrame = pushFrame;
		}
	}

	//=//-----|Setup|---------------------------------------------------------//=//
	public PerformanceSM(Enum psNullOfStateTypeID, object owner)
	{
		//Some basic Setup
		if (psNullOfStateTypeID.ToString() != "PSNull")
		{
			throw new ArgumentException("Performance State Machines must be created with PSNull enum value of desired enum type. ");
		}
		stateIDEnum = psNullOfStateTypeID.GetType();
		this.currentStateID = psNullOfStateTypeID;
		this.owner = owner;

		stateCount = Enum.GetValues(stateIDEnum).Length;
		stateArray = new PerformanceState[stateCount];

		RegisterStates();
	}

	protected PerformanceState GetState(Enum stateID)
	{
		int index = Convert.ToInt32(stateID);

		if (index < 0 || index >= stateArray.Length)
		{
			LogCore.Log("PSM_Error", $"State ID {stateID} is out of bounds.");
			return null;
		}

		return stateArray[index];
	}

	protected void SestState(Enum stateID, PerformanceState state)
	{
		int index = Convert.ToInt32(stateID);

		if (index < 0 || index >= stateArray.Length)
		{
			LogCore.Log("PSM_Error", $"Attempted to set state for {stateID}, but index is out of bounds.");
			return;
		}

		stateArray[index] = state;
	}


	protected virtual void RegisterStates()
	{
		string stateOwnerClassName = owner.GetType().Name;

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
			else
			{
				object stateInstance = Activator.CreateInstance(stateClass);
				SetState(stateID, (PerformanceState)stateInstance);
			}



			//add stateInstance to stateArray

			//how will it be associated with state id for lookup?

		}
	}
	private void ProcessStateHeap()
	{
		// Remove expired state pushes
		while (requestHeap.Count > 0 && requestHeap.Peek().PushLifetime <= currentFrame)
		{
			requestHeap.Dequeue();
		}

		if (requestHeap.Count == 0) return;

		// Get the highest-priority state (max push force, LIFO tie-breaker)
		StateRequest bestRequest = requestHeap.Peek();

		if (currentState == null || bestRequest.PushForce > currentState.GetPriority())
		{
			SwitchToState(bestRequest);
		}
	}
	private void SwitchToState(SetStateRequest newRequest)
	{
		if (stateDictionary.TryGetValue(newRequest.StateName, out var newState))
		{
			currentState = newState;
			currentState.OnEnter();

			// If the new state forces a clear, wipe the heap
			if (currentState.ForceClearStateHeapOnEntry)
			{
				requestHeap.Clear();
			}
			else
			{
				// Remove states where ClearOnStateSwitch = false
				var tempHeap = new MaxHeap<StateRequest>();
				while (requestHeap.Count > 0)
				{
					var request = requestHeap.Dequeue();
					if (request.ClearOnStateSwitch) tempHeap.Enqueue(request, request.PushForce, request.PushFrame);
				}
				requestHeap = tempHeap;
			}
		}
	}
	public void ForceSetCurrentState(Enum stateName)
	{
		if (stateDictionary.TryGetValue(stateName, out var newState))
		{
			currentState = newState;
			currentState.OnEnter();
			requestHeap.Clear();
		}
	}
	//Difinitively set the state 
	private void SetCurrentState(Enum newState)
	{
		string oldState = currentState;
		stateDict[currentState].Exit();
		currentState = newState;
		stateDict[currentState].Enter();

		CLog("CharacterStateHighDetail", $"Switched to from ->{oldState}<- to ->{currentState}<-");


	}

	//Monobehaviour

	public void PSMUpdate()
	{

	}
	public void PSMFixedUpdate()
	{
		currentFrame++;


	}
	public void PSMLateUpdate()
	{

	}
	/

}
