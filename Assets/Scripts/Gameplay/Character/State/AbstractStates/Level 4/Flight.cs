using System;
using UnityEngine;

public class Flight : AirborneState
{
	//Level x state

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	private float flightSpeed = 30.0f;
	private float flightAccFactor = 2.0f;
	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//Functions exlcusive to this member of the state heirarchy

	//=//----------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	//Overrides of the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public Flight(PerformanceCSM sm, Character character) : base(sm, character)
	{
		//...
	}
	protected override void SetStateReferences()
	{
		base.SetStateReferences();
		//...
	}
	public override void SetStateMembers()
	{
		base.SetStateMembers();
		//...
		exitState = CStateID.OO_IdleAirborne;
		allowDrift = true;
		clearFromQueueOnSetState = true;
		forceClearQueueOnEntry = true;
		priority = 0;
		stateDuration = 0;
		minimumStateDuration = ch.stdMinStateDuration;
		exitOnStateComplete = false;



	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected override void ProcessInput()
	{
		base.ProcessInput();
		//...
	}
	protected override void SetOnEntry()
	{
		base.SetOnEntry();
		//...
	}
	protected override void PerFrame()
	{
		base.PerFrame();
		//...
	}
	#endregion data_management
	//=//-----|Routing|--------------------------------------------------//=//
	#region routings
	protected override void RouteState()
	{
		//...
		base.RouteState();
	}
	protected override void RouteStateFixed()
	{
		//...
		base.RouteStateFixed();
	}
	#endregion routing
	//=//-----|Flow|-----------------------------------------------------//=//
	#region flow
	public override void Enter()
	{
		base.Enter();
		//...
	}
	public override void Exit()
	{
		base.Exit();
		//...
	}
	#endregion flow
	//=//-----|Mono|-----------------------------------------------------//=//
	#region mono
	public override void Update()
	{
		base.Update();
		//...
	}
	public override void FixedFrameUpdate()
	{
		//...
		base.FixedFrameUpdate();
	}
	public override void FixedPhysicsUpdate()
	{
		//...
		base.FixedPhysicsUpdate();
	}
	public override void LateUpdate()
	{
		//...
		base.LateUpdate();
	}
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
	
	protected override void ApplyGravity()
	{
		//do nothin
	}
	//=//----------------------------------------------------------------//=//
	#endregion level_2
	/////////////////////////////////////////////////////////////////////////////





	//======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_3

	protected override void HandleDrift()
	{
		Vector3 tv = ch.inputMoveDirection;
		tv *= flightSpeed;

		AddForceByTargetVelocity("Flight", tv, flightAccFactor);
	}
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
