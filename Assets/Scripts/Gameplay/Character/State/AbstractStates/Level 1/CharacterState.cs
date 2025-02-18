using System;
using UnityEngine;

public class CharacterState : PerformanceState
{
	//Level 1 Abstract State

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	[Header("Parent")]
	protected Character ch;

	[Header("Meta")]
	public CStateID stateType;

	[Header("Component Refs")]
	protected Animator anim;
	protected Rigidbody rb;
	protected CapsuleCollider cc;
	protected PlayerInputHandler pinput;
	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	//Methods from the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public CharacterState(PerformanceSM sm, Character character) : base(sm)
	{
		this.ch = character;

		SetStateReferences();
		SetOnEntry();

	}
	protected override void SetStateReferences()
	{
		base.SetStateReferences();

		this.anim = ch.animator;
		this.rb = ch.rigidBody;
		this.cc = ch.capsuleCollider;
		this.pinput = ch.inputHandler;
	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected override void SetOnEntry()
	{
		this.priority = 3; //HACK: simple for now
		minimumStateDuration = 5; //HACK: simple for now
	}
	protected override void PerFrame()
	{
		base.PerFrame();
		//...
	}
	#endregion data_management
	//=//-----|Routing|--------------------------------------------------//=//
	#region routing
	protected override void StatePushState(Enum stateID, int pushForce, int lifeTime)
	{
		ch.StatePushState(stateID, pushForce, lifeTime);
	}
	protected override void TryRouteState()
	{
		//...
		base.TryRouteState();
	}
	protected override void TryRouteStateFixed()
	{
		//...
		base.TryRouteStateFixed();
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
	#region level_1
	//=//----------------------------------------------------------------//=//
	#endregion level_1
	/////////////////////////////////////////////////////////////////////////////





	//======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_1
	//=//----------------------------------------------------------------//=//
	#endregion level_1
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_1
	//=//----------------------------------------------------------------//=//
	#endregion level_1
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_1
	//=//----------------------------------------------------------------//=//
	#endregion level_1
	/////////////////////////////////////////////////////////////////////////////
}
