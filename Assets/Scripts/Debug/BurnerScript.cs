using System;
using UnityEngine;

public abstract class PerformanceState22
{
	//THE BASE STATE

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	//meta
	public string stateName;
	//refs
	public PerformanceSM stateMachine;
	//Flow
	public Enum exitState;
	public bool clearOnSetState;
	public bool forceClearStateHeapOnEntry;
	public int priority;
	public int stateDuration; //0, if indefinite 
	public int minimumStateDuration; //anti fluttering
	public int currentFrame;
	public bool exitAllowed; //overules priority
	public bool stateComplete;
	public bool exitOnStateComplete;
	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////

	//======// /==/==/==/=||[LOCAL]||==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//Methods for only this class 

	#endregion local
	/////////////////////////////////////////////////////////////////////////////

	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	//Methods from the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public PerformanceState(PerformanceSM sm)
	{
		this.stateName = GetType().Name;
		this.stateMachine = sm;
	}
	protected override void SetStateReferences()
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
	#endregion data_management
	//=//-----|Routing|--------------------------------------------------//=//
	#region routings
	protected virtual void TryRouteState()
	{
		
	}
	protected virtual void TryRouteStateFixed()
	{


		//...
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
	public virtual void FixedPhysicsUpdate() { }
	public override void LateUpdate() { }
	#endregion mono
	//=//-----|Debug|----------------------------------------------------//=//
	#region debug
	public override bool VerifyState()
	{
		return base.VerifyState();
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
