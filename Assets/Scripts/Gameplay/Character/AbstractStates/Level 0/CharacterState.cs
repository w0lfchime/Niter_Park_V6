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


    //=//-----|Flow Control|------------------------------------------//=//
    [Header("Parameters")] //All params could and may be changed at runtime. who knows.
	public bool exitOnExitAllowed; //set true if state does not have particular exit routing
	public string defaultExitState; //could also be used as a variable, but under parameter for general clarity
	public float minimumStateDuration;
    public bool forceClearStateHeapOnEntry;

	[Header("Variables")]
	protected bool exitAllowed;
	protected float stateEntryTimeStamp;






	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//


	//=//-----|Setup|------------------------------------------------//=//
	public CharacterState(Character character) : base()
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
        this.priority();

    }


    //=//-----|Data Management|-------------------------------------//=//
    /// <summary>
    /// Base: Called in Enter().
    /// </summary>
    protected virtual void SetVariablesOnEntry()
    {
        stateEntryTimeStamp = Time.time;
    }


    //=//-----|Flow Control|---------------------------------------//=//
    public override void Enter()
    {
        base.Enter();

        SetVariablesOnEntry();

        LogCore.Log("CharacterStateFlow", $"Entering State {stateName}");
    }
    public override void Exit()
    {
        base.Exit();

        LogCore.Log("CharacterStateFlow", $"Exting State {stateName}");
    }
    protected virtual void CheckExitAllowed()
    {
        if (!(Time.time - stateEntryTimeStamp > minimumStateDuration))
        {
            exitAllowed = false;
        }
    }
    protected virtual void TryRouteState()
    {
        if (exitOnExitAllowed)
        {
            ch.PushState(defaultExitState, 1);
        }
    }
    protected virtual void TryRouteStateFixed()
    {

    }


    //=//-----|MonoBehavior|---------------------------------------//=//
    public override void Update()
    {
        base.Update();

        CheckExitAllowed();

        if (exitAllowed)
        {
            TryRouteState();
        }
    }
    public override void FixedUpdate()
    {
        base.Update();

        if (exitAllowed)
        {
            TryRouteStateFixed();
        }
    }
    public override void LateUpdate()
    {
        base.LateUpdate();


    }







}
