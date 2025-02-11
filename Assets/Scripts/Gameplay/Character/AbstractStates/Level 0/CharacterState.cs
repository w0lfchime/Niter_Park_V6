using UnityEngine;

public abstract class CharacterState
{
	[Header("Parent")]
	protected Character ch;

	[Header("Meta")]
	public string stateName;

	[Header("Component Refs")]
	protected Animator anim;
	protected Rigidbody rb;
	protected CapsuleCollider cc;
	protected PlayerInputHandler pinput;

	[Header("Time")]
	protected float minimumStateDuration;
	protected float stateEntranceTimeStamp;

	[Header("Flow Control Parameters")]
	protected bool exitOnExitAllowed; //set true if state does not have particular exit routing
    protected string defaultExitState; //could also be used as a variable, but under parameter for general clarity

    [Header("Flow Control Variables")]
	protected bool exitAllowed;


	public CharacterState(Character character)
	{
		//refs
		this.ch = character;
		this.anim = character.animator;
		this.rb = character.rigidBody;
		this.cc = character.capsuleCollider;
		this.pinput = character.inputHandler;

		//meta 
		this.stateName = GetType().Name;

		//flow control
		this.defaultExitState = "IdleAirborne";
		this.exitOnExitAllowed = false;
	}

	public virtual void Enter() 
	{
		exitAllowed = false;
		stateEntranceTimeStamp = Time.time;

		LogCore.Log("StateTransition", $"Entering State {stateName}");
	}
	public virtual void Exit() 
	{
		exitAllowed = false; //for redundancy 

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

        if (!(Time.time - stateEntranceTimeStamp > minimumStateDuration))
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

}

