using System;
using TMPro.Examples;
using UnityEngine;


public class PerformanceSM
{
	//======// /==/==/==/=||[STATE MACHINE]||==/==/==/==/==/==/==/==/==/==/==/ //======//
	//=//-----|Fields|----------------------------------------------------------//=//

	public Type elementEnumType;
	public int currentState;


	protected PerformanceState[] stateArray = new PerformanceState[(int)CState.COUNT];
	protected StateSetRequestHeap<StateSetRequest> stateHeap = new();

	public int perFrameStateQueueLimit = 10;

	protected struct StateSetRequest
	{
		public string StateName;
		public int PushForce;
		public int ExpirationFrame;
		public bool ClearOnStateSwitch;
		public int PushFrame; // Used for LIFO sorting

		public StateSetRequest(string stateName, int pushForce, int expirationFrame, bool clearOnStateSwitch, int pushFrame)
		{
			StateName = stateName;
			PushForce = pushForce;
			ExpirationFrame = expirationFrame;
			ClearOnStateSwitch = clearOnStateSwitch;
			PushFrame = pushFrame;
		}
	}

	private void ResgisterState()
	{

	}

	protected virtual void RegisterStates()
	{


		foreach (CState stateEnum in Enum.GetValues(typeof(CState)))
		{
			string className = $"{this.characterName}{stateEnum}";  // "GenericIdleAirborne", etc.
			Type classType = Type.GetType(className);

			if (classType != null)
			{
				CharacterState stateInstance = (CharacterState)Activator.CreateInstance(classType, this);
				RegisterState(stateEnum, stateInstance);
			}
			else
			{
				LogCore.Log("CharacterSetup", $"State class not found: {className}");
			}
		}
	}
	private void ProcessStateHeap()
	{
		// Remove expired state pushes
		while (stateHeap.Count > 0 && stateHeap.Peek().ExpirationFrame <= currentFixedFrame)
		{
			stateHeap.Dequeue();
		}

		if (stateHeap.Count == 0) return;

		// Get the highest-priority state (max push force, LIFO tie-breaker)
		StateRequest bestRequest = stateHeap.Peek();

		if (currentState == null || bestRequest.PushForce > currentState.GetPriority())
		{
			SwitchToState(bestRequest);
		}
	}
	private void SwitchToState(StateSetRequest newRequest)
	{
		if (stateDictionary.TryGetValue(newRequest.StateName, out var newState))
		{
			currentState = newState;
			currentState.OnEnter();

			// If the new state forces a clear, wipe the heap
			if (currentState.ForceClearStateHeapOnEntry)
			{
				stateHeap.Clear();
			}
			else
			{
				// Remove states where ClearOnStateSwitch = false
				var tempHeap = new MaxHeap<StateRequest>();
				while (stateHeap.Count > 0)
				{
					var request = stateHeap.Dequeue();
					if (request.ClearOnStateSwitch) tempHeap.Enqueue(request, request.PushForce, request.PushFrame);
				}
				stateHeap = tempHeap;
			}
		}
	}
	public void ForceState(string stateName)
	{
		if (stateDictionary.TryGetValue(stateName, out var newState))
		{
			currentState = newState;
			currentState.OnEnter();
			stateHeap.Clear();
		}
	}
	//Finally,
	private void SetState(string newState)
	{
		string oldState = currentState;
		stateDict[currentState].Exit();
		currentState = newState;
		stateDict[currentState].Enter();

		CLog("CharacterStateHighDetail", $"Switched to from ->{oldState}<- to ->{currentState}<-");


	}
}
