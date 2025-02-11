using System;
using System.Reflection;
using UnityEngine;



public abstract class CharacterState
{
	//Cor

	[Header("Parent")]
	protected Character ch;

	[Header("Meta")]
	public string stateName;

	[Header("Component Refs")]
	protected Animator anim;
	protected Rigidbody rb;
	protected CapsuleCollider cc;
	protected PlayerInputHandler pinput;

	//Parameters and Variables 

	[Header("Flow Control Parameters")]
	protected bool exitOnExitAllowed; //set true if state does not have particular exit routing
	protected string defaultExitState; //could also be used as a variable, but under parameter for general clarity
	protected float minimumStateDuration;

	[Header("Flow Control Variables")]
	protected bool exitAllowed;
	protected float stateEntryTimeStamp;



	public CharacterState(Character character)
	{
		this.ch = character;
	
		SetStateReferences();
		SetStateParameters();

	}
	protected virtual void SetStateReferences()
	{
		this.anim = ch.animator;
		this.rb = ch.rigidBody;
		this.cc = ch.capsuleCollider;
		this.pinput = ch.inputHandler;
	}
	protected virtual void SetStateParameters()
	{
		this.stateName = GetType().Name;
	}

	/// <summary>
	/// Called on state entry by base CharacterState class. 
	/// Variables to set on entry:
	/// bool exitAllowed,
	/// ...and more
	/// </summary>
	protected virtual void SetStateVariablesOnEntry()
	{
		stateEntryTimeStamp = Time.time;

	}

	public virtual void Enter() 
	{
		SetStateVariablesOnEntry();

		LogCore.Log("CharacterStateFlow", $"Entering State {stateName}");
	}
	public virtual void Exit() 
	{
		LogCore.Log("CharacterStateFlow", $"Exting State {stateName}");
	}
	public virtual void Update() 
	{
		CheckExitAllowed();

		if (exitAllowed)
		{
			TryRouteState();
		}



	}
	public virtual void FixedUpdate() 
	{
		if (exitAllowed)
		{
			TryRouteStateFixed();
		}


	}

	public virtual void LateUpdate()
	{

	}

	/// <summary>
	/// Exit allowed must be set true somewhere. 
	/// </summary>
	protected virtual void CheckExitAllowed()
	{
		//or base.CheckExitAllowed call
		//exitAllowed checks that should be overridden by min state time

		if (!(Time.time - stateEntryTimeStamp > minimumStateDuration))
		{
			exitAllowed = false;
		}

		//or base.CheckExitAllowed call
		//exitAllowed checks that should override min state time 
	}


	/// <summary>
	/// State routing. Is called in update when exitAllowed is true. 
	/// </summary>
	protected virtual void TryRouteState()
	{

		if (exitOnExitAllowed)
		{
			ch.TrySetState(defaultExitState, 1);
		}


	}

	/// <summary>
	/// Just like TryRouteState(), but on fixed update. Useful for percise physics related transistions.
	/// </summary>
	protected virtual void TryRouteStateFixed()
	{

	}

	//HACK: data copy method ? (for data in character states, probably not.)



}

