using UnityEngine;
using System.Reflection;
using System;

public abstract class CharacterState
{
	//Core 

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
	
		SetReferences();
		SetStateParameters();
		SetStateVariablesOnEntrance(); //for 

		TestAllMemberFieldsForNull();
	}

	/// <summary>
	/// Called in the CharacterState base constructor. 
	/// Sets references of a character state for more immediate access. 
	/// More references can be set in child override if needed.
	/// Sets:
	/// animator as anim,
	/// rigidbody as rb, 
	/// capsuleCollider as cc,
	/// inputHandler as pinput,
	/// </summary>
	public virtual void SetReferences()
	{
		this.anim = ch.animator;
		this.rb = ch.rigidBody;
        this.cc = ch.capsuleCollider;
		this.pinput = ch.inputHandler;
	}
    /// <summary>
    /// Called in base CharacterState constructor.
    /// Paramters that should be set on construction: 
    /// string defaultExitState, 
    /// bool exitOnExitAllowed, 
    /// float minimumStateDuration,
    /// ...and more
    /// 
    /// </summary>
    public virtual void SetStateParameters()
	{
		this.stateName = GetType().Name;
	}

	/// <summary>
	/// Called on state entry by base CharacterState class. 
	/// Variables to set on entry:
	/// bool exitAllowed,
	/// ...and more
	/// </summary>
	public virtual void SetStateVariablesOnEntrance()
	{
        stateEntryTimeStamp = Time.time;

    }

	public virtual void Enter() 
	{
		SetStateVariablesOnEntrance();

		LogCore.Log("StateTransition", $"Entering State {stateName}");
	}
	public virtual void Exit() 
	{
        LogCore.Log("StateTransition", $"Exting State {stateName}");
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

	/// <summary>
	/// Exit allowed must be set true somewhere. 
	/// </summary>
	public virtual void CheckExitAllowed()
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
	public virtual void TryRouteState()
	{

		if (exitOnExitAllowed)
		{
			ch.TrySetState(defaultExitState, 1);
		}


	}

	/// <summary>
	/// Just like TryRouteState(), but on fixed update. Useful for percise physics related transistions.
	/// </summary>
	public virtual void TryRouteStateFixed()
	{

	}

    //HACK: data copy method ? (for data in character states, probably not.)

    //DEBUG FUNCTIONS

	/// <summary>
	/// Test that checks all member variables to ensure they're set.
	/// </summary>
    public virtual void TestAllMemberFieldsForNull()
    {
        Type type = this.GetType(); // Get the runtime type (most derived type)

        while (type != null && type != typeof(object)) // Iterate through all ancestor types
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(this);
                if (value == null)
                {
					LogCore.Log("Critical", $"CharacterState ERROR in class: {GetType().Name}");
					LogCore.Log("Critical", $"Field {field.GetType().Name} {field.Name} member of {type.Name} is null.");
                    LogCore.Log("Critical", $"Ensure that the value is properly set in {GetType().Name}");
                }
            }

            type = type.BaseType; // Move to the parent class
        }
    }

}

