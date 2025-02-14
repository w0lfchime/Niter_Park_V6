using System;
using System.Reflection;
using UnityEngine;

public abstract class CharacterState
{
    //======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//


    //=//-----|General|----------------------------------------------//=//
    [Header("Parent")]
    protected Character ch;

    [Header("Meta")]
    public string stateName;
    public CState stateType;

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
	public int statePriority;
	protected bool exitAllowed;
	protected float stateEntryTimeStamp;






	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//


	//=//-----|Setup|------------------------------------------------//=//
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


    //=//-----|Data Management|-------------------------------------//=//
    /// <summary>
    /// Base: Called in Enter().
    /// </summary>
    protected virtual void SetVariablesOnEntry()
    {
        stateEntryTimeStamp = Time.time;
    }


    //=//-----|Flow Control|---------------------------------------//=//
    public virtual void Enter()
    {
        SetVariablesOnEntry();

        LogCore.Log("CharacterStateFlow", $"Entering State {stateName}");
    }
    public virtual void Exit()
    {
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
        //nothing yet SHRUG
    }


    //=//-----|MonoBehavior|---------------------------------------//=//
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
        //nothing yet....
    }


    //=//-----|Debug|---------------------------------------------//=//
    public virtual bool VerifyState()
    {
        return CheckStateForNullFields();
    }
    protected bool CheckStateForNullFields()
    {
        bool passed = true;

        Type type = this.GetType();

        while (type != null && type != typeof(object))
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(this);
                if (value == null)
                {
                    passed = false;
                    string message = $"Critical error in {ch.characterName}. ";
                    string message2 = $"Field {field.Name} is null, member of state {type.Name} of the state heirarchy.";
                    LogCore.Log("CriticalError", message + message2);
                }
            }

            type = type.BaseType;
        }

        return passed;
    }

	//=//-----|Get & Set|---------------------------------------------//=//
    public int getPriority()
    {
        return statePriority;
    }

}
