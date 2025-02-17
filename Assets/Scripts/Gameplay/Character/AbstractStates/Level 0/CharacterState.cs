using System;
using System.Reflection;
using UnityEngine;

public class CharacterState : PerformanceState
{
	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//

	//=//-----|General|----------------------------------------------//=//
	[Header("Parent")]
	protected Character ch;

	[Header("Meta")]
	public CStateID stateType;

	[Header("Component Refs")]
	protected Animator anim;
	protected Rigidbody rb;
	protected CapsuleCollider cc;
	protected PlayerInputHandler pinput;



	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//

	//=//-----|Setup|------------------------------------------------//=//
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


	//=//-----|Data Management|-------------------------------------//=//
	/// <summary>
	/// Base: Called in Enter().
	/// </summary>
	protected override void SetOnEntry()
	{
		this.priority = 1;
		minimumStateDuration = 5;
	}




	//=//-----|Flow|---------------------------------------//=//
	public override void Enter()
	{
		base.Enter();

		SetOnEntry();
	}
	public override void Exit()
	{
		base.Exit();


	}



	//=//-----|MonoBehavior|---------------------------------------//=//
	public override void Update()
	{
		base.Update();

		//...

		if (exitAllowed)
		{
			TryRouteState();
		}
	}
	public override void FixedFrameUpdate()
	{
		base.FixedFrameUpdate();

		//...

		if (exitAllowed)
		{
			TryRouteStateFixed();
		}
	}
	public override void FixedPhysicsUpdate()
	{
		base.FixedPhysicsUpdate();

		//...
	}
	public override void LateUpdate()
	{
		base.LateUpdate();

		//...
	}
	//======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======//
	//=//-----|Routing|-------------------------------------------------//=//

	//=//---------------------------------------------------------------//=//
}