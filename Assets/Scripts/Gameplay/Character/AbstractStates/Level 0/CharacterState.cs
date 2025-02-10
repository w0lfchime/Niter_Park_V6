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


        LogCore.Log("StateTransition", $"Exting State {stateName}");
    }
    public virtual void Update() { }
    public virtual void FixedUpdate() 
    {
        CheckExitAllowed();


        TryRouteState();
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
        if (exitAllowed)
        {
            if (exitOnExitAllowed)
            {
                ch.TrySetState(defaultExitState, 1);
            }
        }

    }

    //data copy method ?

}

