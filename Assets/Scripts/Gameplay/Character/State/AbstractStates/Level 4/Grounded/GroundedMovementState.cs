using System;
using UnityEditor.Build;
using UnityEngine;

public enum GLState
{
	Idle,
	Walking,
	Running,
	SneakIdle,
	SneakMove,
	Other
}

public class GroundedMovementState : GroundedState
{
	//Level x state

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields

	public GLState currGLState;

	private float maxGLSpeedOverflow = 0.0004f;

	private bool holdSneak;
	private bool holdSprint;

	private float currSpeed;
	private float currAccFactor;

	private bool isLocomotion;
	private bool onEnterLocomotion;
	private bool onExitLocomotion;
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
	public GroundedMovementState(PerformanceCSM sm, Character character) : base(sm, character)
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
		exitState = CStateID.OO_GroundedMovement; //clearly, itself
		clearFromQueueOnSetState = true;
		forceClearQueueOnEntry = false;
		priority = 1;
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

		holdSprint = ih.GetButtonHold("Run");
		holdSneak = ih.GetButtonHold("Sneak");

		if (holdSprint && holdSneak)
		{
			holdSprint = false;
			holdSneak = false;
		}
	}
	protected override void SetOnEntry()
	{
		base.SetOnEntry();
		//...
		currGLState = GLState.Other;
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
        aapc.SetAnimatorState(STDAnimState.GroundedIdle, 0.1f);
        DetermineGLState(); //get an extra on in there
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
        DetermineGLState();
    }
	public override void FixedFrameUpdate()
	{
		HandleGLStateAndData();
		AnimationData();
        //...
        base.FixedFrameUpdate();
	}
	public override void FixedPhysicsUpdate()
	{
		HandleLocomotionForce();
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
    //=//----------------------------------------------------------------//=//
    #endregion level_2
    /////////////////////////////////////////////////////////////////////////////



    //======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//
    #region level_3
    //=//----------------------------------------------------------------//=//
    #endregion level_3
    /////////////////////////////////////////////////////////////////////////////




    //======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
    #region level_4
    //=//------|Locomotion|----------------------------------------------//=//
	private void DetermineGLState()
	{
		bool moveInput = ch.inputMoveDirection.x != 0;

		if (holdSneak)
		{
			currGLState = moveInput ? GLState.SneakMove : GLState.SneakIdle;
		}
		else if (holdSprint)
		{
			currGLState = moveInput ? GLState.Running : GLState.Idle;
		}
		else
		{
			currGLState = moveInput ? GLState.Walking : GLState.Idle;
		}
	}

	private void HandleGLStateAndData()
	{
		this.currAccFactor = ch.acs.gAccFactor;

		onEnterLocomotion = false;
		onExitLocomotion = false;
		bool oldLocoState = isLocomotion;

		switch (currGLState)
		{
			case GLState.Idle:
				currSpeed = 0;
				isLocomotion = false;
				break;
            case GLState.Walking:
				isLocomotion = true;
				currSpeed = ch.acs.gWalkSpeed;
                break;
            case GLState.Running:
				isLocomotion = true;
				currSpeed = ch.acs.gRunSpeed;
                break;
            case GLState.SneakIdle:
				isLocomotion = false;
				currSpeed = 0;
                break;
            case GLState.SneakMove:
				isLocomotion = false;
				currSpeed = ch.acs.gSneakSpeed;
                break;
			case GLState.Other:
				isLocomotion = false;
				break;
			default:
				break;
		}

		if (isLocomotion != oldLocoState)
		{
			if (isLocomotion)
			{
				onEnterLocomotion = true;
			}
			else
			{
				onExitLocomotion = true;
			}
		}
	}

	private void HandleLocomotionForce()
	{
		Vector3 tv = ch.inputMoveVectorRaw;
		tv.y = 0;
		tv *= currSpeed;

		AddForceByTargetVelocity("gLocomotion", tv, currAccFactor);
	}

	private void AnimationData()
	{
		//if (onEnterLocomotion)
		//{
		//	aapc.SetAnimatorState(STDAnimState.GroundedLocomotion);
		//}
		//else if (onExitLocomotion)
		//{
		//	aapc.SetAnimatorState(STDAnimState.GroundedIdle);
		//}



        if (isLocomotion)
		{
			aapc.SoftSetAnimatorState(STDAnimState.GroundedLocomotion);

			float currentSpeed = ch.characterSpeed;
			float maxSpeed = ch.acs.gRunSpeed;
			float animGLSpeed = currentSpeed / maxSpeed;
			float overDrive = Mathf.Clamp01(currentSpeed / currSpeed);

			if (animGLSpeed < 1 + maxGLSpeedOverflow)
			{
				aapc.animator.SetFloat("GLSpeed", animGLSpeed);
				aapc.animator.SetFloat("GLOverdrive", overDrive);
			}
			else
			{
				//MORE overdrive
			} 
		} 
		else
		{
			aapc.SoftSetAnimatorState(STDAnimState.GroundedIdle);
		}
	}
    //=//----------------------------------------------------------------//=//
    #endregion level_4
    /////////////////////////////////////////////////////////////////////////////




    //======// /==/==/==/==||[LEVEL 5]||==/==/==/==/==/==/==/==/==/==/ //======//
    #region level_5
    //=//----------------------------------------------------------------//=//
    #endregion level_5
    /////////////////////////////////////////////////////////////////////////////
}
