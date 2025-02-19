using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PerformanceState
{
	//THE BASE STATE

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	//meta
	public string stateName;
	//refs
	public PerformanceCSM stateMachine;
	//Flow Params
	public CStateID exitState;
	public bool clearOnSetState;
	public bool forceClearStateHeapOnEntry;
	public int priority;
	public int stateDuration; //0, if indefinite 
	public int minimumStateDuration; //anti fluttering
	public bool exitOnStateComplete;
	//Flow variables
	public int currentFrame;
	public bool exitAllowed; //overules priority
	public bool stateComplete;







	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//Methods for only this class 
	//=//-----|Get & set|------------------------------------------------//=//
	#region get_and_set
	public int GetPriority()
	{
		return priority;
	}
	#endregion get_and_set
	//=//-----|Debug|----------------------------------------------------//=//
	#region debug
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
	#endregion debug
	//=//----------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	//Methods from the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public PerformanceState(PerformanceCSM sm)
	{
		this.stateName = GetType().Name;
		this.stateMachine = sm;
	}
	protected virtual void SetStateReferences()
	{
		//...
	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected virtual void SetOnEntry()
	{
		exitAllowed = false;
		currentFrame = 0;
	}
	protected virtual void PerFrame()
	{
		currentFrame++;

		if (currentFrame < minimumStateDuration)
		{
			exitAllowed = false;
			stateComplete = false;
		}
		if (stateDuration != 0 && currentFrame >= stateDuration)
		{
			stateComplete = true;
		}

		//...
	}
	#endregion data_management
	//=//-----|Routing|--------------------------------------------------//=//
	#region routing
	protected abstract void StatePushState(CStateID stateID, int pushForce, int lifetime); //for push state
	protected virtual void RouteState()
	{
		//...
		if (exitOnStateComplete && stateComplete)
		{
			StatePushState(exitState, 2, 2);
		}
	}
	protected virtual void RouteStateFixed()
	{

	}
	#endregion routing
	//=//-----|Flow|-----------------------------------------------------//=//
	#region flow
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
	#endregion flow
	//=//-----|Mono|-----------------------------------------------------//=//
	#region mono
	public virtual void Update()
	{
		//...
		RouteState();
	}
	public virtual void FixedFrameUpdate()
	{
		PerFrame();
		//...
		RouteStateFixed();
	}
	public virtual void FixedPhysicsUpdate() { }
	public virtual void LateUpdate() { }
	#endregion mono
	//=//-----|Debug|----------------------------------------------------//=//
	#region debug
	public virtual bool VerifyState()
	{
		return CheckStateForNullFields();
	}
	#endregion debug
	//=//----------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_1
	//=//----------------------------------------------------------------//=//
	#endregion level_1
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_2
	//=//----------------------------------------------------------------//=//
	#endregion level_2
	/////////////////////////////////////////////////////////////////////////////





	//======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_3
	//=//----------------------------------------------------------------//=//
	#endregion level_3
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_4
	//=//----------------------------------------------------------------//=//
	#endregion level_4
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 5]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_5
	//=//----------------------------------------------------------------//=//
	#endregion level_5
	/////////////////////////////////////////////////////////////////////////////
}
