using System;
using System.Reflection;
using UnityEngine;

public abstract class PerformanceState
{
	//meta
	public string stateName;


	//refs
	public PerformanceSM stateMachine;

	//Flow
	public Enum exitState; 
	public bool clearOnSetState;
	public bool forceClearStateHeapOnEntry;
	public int priority;


	//Timing
	public int currentFrame;
	public bool exitAllowed; //overules priority
	public int stateDuration; //0, if indefinite 
	public int minimumStateDuration; //anti fluttering
	public bool stateComplete;
	public bool exitOnStateComplete;



	public PerformanceState(PerformanceSM sm)
	{
		this.stateName = GetType().Name;
		this.stateMachine = sm;
	}

	//Data

	protected virtual void SetStateReferences()
	{
		//...
	}

	protected virtual void SetOnEntry()
	{
		exitAllowed = false;
		currentFrame = 0;

		if (stateDuration == 0)
		{
			exitOnStateComplete = false;
		}
	}


	//Time
	/// <summary>
	/// frame related processes and decision
	/// </summary>
	protected virtual void PerFrame()
	{
		currentFrame++;

		if (currentFrame > minimumStateDuration)
		{
			exitAllowed = true;
			stateComplete = false;
		}
		if (stateDuration == 0 || currentFrame >= stateDuration)
		{
			stateComplete = true;
		}

		//...
	}



	//General

	public virtual void Enter()
    {
		LogCore.Log("PSM_Flow", $"Entering State {stateName}.");

		SetOnEntry();

		//...
    }
    public virtual void Exit()
    {
		LogCore.Log("PSM_Flow", $"Exting State {stateName}.");

		//...
	}
    public virtual void Update()
    {
		//...

		if (exitAllowed)
		{
			TryRouteState();
		}
	}
    public virtual void FixedFrameUpdate()
    {
		PerFrame();

		//...

		if (exitAllowed)
		{
			TryRouteStateFixed();
		}
    }
	public virtual void FixedPhysicsUpdate()
	{

	}
    public virtual void LateUpdate()
    {

    }
	//=//-----|Routing|---------------------------------------------//=//

	protected virtual void TryRouteState()
	{
		//...
		if (exitOnStateComplete && stateComplete)
		{
			
		}
	}
	protected virtual void TryRouteStateFixed()
	{


		//...
	}






	//=//-----|Get & Set|---------------------------------------------//=//
	public int GetPriority()
	{
		return priority;
	}

	

	//=//-----|Debug|---------------------------------------------//=//
	public virtual bool VerifyState()
	{
		return CheckStateForNullFields();
	}

	protected bool CheckStateForNullFields()
	{
		bool passed = true;

		Type type = this.GetType();

		while (type != null && type != typeof(object))
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(this);
				if (value == null)
				{
					passed = false;
					string message = $"Field {field.Name} is null, member of state {type.Name} of the state heirarchy.";
					LogCore.Log("CriticalError", message);
				}
			}

			type = type.BaseType;
		}

		return passed;
	}
}
