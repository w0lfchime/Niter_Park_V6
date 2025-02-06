using UnityEngine;

public abstract class CharacterState
{
    [Header("Parent")]
    protected Character ch;

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

    [Header("Flow Control Variables")]
    protected string defaultExitState;
    protected bool exitAllowed;


    public CharacterState(Character character)
    {
        //refs
        this.ch = character;
        this.anim = character.animator;
        this.rb = character.rigidBody;
        this.cc = character.capsuleCollider;
        this.pinput = character.inputHandler;

        //flow control
        this.defaultExitState = "IdleAirborne";
        this.exitOnExitAllowed = false;
    }

    public virtual void Enter() 
    {
        exitAllowed = false;
        stateEntranceTimeStamp = Time.time;
    }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() 
    {
        CheckExitAllowed();

        if (exitAllowed)
        {
            TryRouteState();
        }
    }

    /// <summary>
    /// Exit allowed must be set true somewhere. 
    /// </summary>
    public virtual void CheckExitAllowed()
    {
        if (!(Time.time - stateEntranceTimeStamp > minimumStateDuration))
        {
            exitAllowed = false;
        }

        //more requirements 
    }


    /// <summary>
    /// State routing. Base/Parent virtual will 
    /// </summary>
    public virtual void TryRouteState()
    {
        if (exitOnExitAllowed)
        {
            ch.TrySetState(defaultExitState, 1);
        }
    }

    //data copy method ?

}

