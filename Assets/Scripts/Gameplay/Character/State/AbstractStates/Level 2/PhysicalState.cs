using System;
using UnityEngine;

public class PhysicalState : CharacterState
{
	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	private int onGroundingHoldFrames = 5;
	private int onUngroundingHoldFrames = 5;
	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//Functions exlcusive to this member of the state heirarchy

	//=//----------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======// PERFORMANCE STATE
	//Overrides of the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public PhysicalState(PerformanceCSM sm, Character character) : base(sm, character)
	{
		//...
	}
	protected override void SetStateReferences()
	{
		base.SetStateReferences();
		//...
	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected override void SetOnEntry()
	{
		base.SetOnEntry();
		//...
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
		HandleGrounding();
		HandleJump();
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
		PhysicalDataUpdates();
	}
	public override void FixedFrameUpdate()
	{
		//...
		base.FixedFrameUpdate();
	}
	public override void FixedPhysicsUpdate()
	{
		WatchGrounding();
		SetGrounding();
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




	//======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======// CHARACTER STATE

	#region level_1
	//=//----------------------------------------------------------------//=//
	#endregion level_1
	/////////////////////////////////////////////////////////////////////////////





	//======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======// PHYSICAL STATE
	#region level_2
	//=//-----|Data Management|-----------------------------------------//=//
	#region data_management
	protected virtual void PhysicalDataUpdates()
	{
		ch.position = ch.transform.position;

		Vector3 lv = rb.linearVelocity;
		ch.velocity = lv;
		ch.velocityX = lv.x;
		ch.velocityY = lv.y;
		ch.playerSpeed = lv.magnitude;

		//debug 
		ch.UpdateDebugVector("Velocity", lv, Color.green);

	}
	#endregion data_management
	//=//-----|Force|---------------------------------------------------//=//
	#region force
	public virtual void AddForce(string forceName, Vector3 force)
	{
		ch.UpdateDebugVector(forceName, force, Color.yellow);

		ch.appliedForce += force;
	}
	public virtual void AddImpulseForce(string forceName, Vector3 impulseForce)
	{
		ch.StampDebugVector(forceName, impulseForce, Color.red);
		ch.appliedImpulseForce += impulseForce;
	}
	protected virtual void AddForceByTargetVelocity(string forceName, Vector3 targetVelocity, float forceFactor)
	{
		//debug
		string tvName = $"{forceName}_TargetVelocity";
		ch.UpdateDebugVector(tvName, targetVelocity, Color.white);

		//force
		Vector3 forceByTargetVeloity = Vector3.zero;
		forceByTargetVeloity += targetVelocity - ch.velocity;
		forceByTargetVeloity *= forceFactor;
		AddForce(forceName, forceByTargetVeloity);
	}
	protected virtual void ApplyGravity()
	{
		Vector3 gravForceVector = Vector3.up * ch.acd.gravityTerminalVelocity;
		AddForce("Gravity", gravForceVector);
	}
	#endregion force
	//=//-----|Grounding|-----------------------------------------------//=//
	#region grounding
	protected virtual void WatchGrounding()
	{
		float sphereRadius = cc.radius;
		Vector3 capsuleRaycastStart = ch.transform.position + new Vector3(0, sphereRadius + 0.1f, 0);

		UnityEngine.Debug.DrawRay(capsuleRaycastStart, Vector3.down * ch.acd.groundCheckingDistance, Color.red);
		UnityEngine.Debug.DrawRay(capsuleRaycastStart + new Vector3(0.1f, 0, 0), Vector3.down * ch.acd.isGroundedDistance, Color.blue);

		RaycastHit hit;

		if (Physics.SphereCast(capsuleRaycastStart, sphereRadius, Vector3.down, out hit, ch.acd.groundCheckingDistance, ch.groundLayer))
		{
			ch.distanceToGround = hit.distance - sphereRadius;
		}
		else
		{
			ch.distanceToGround = ch.acd.groundCheckingDistance;
		}


	}
	public void SetGrounding()
	{
		bool groundedByDistance = ch.distanceToGround < ch.acd.isGroundedDistance;

		if (groundedByDistance != ch.isGrounded)
		{
			if (Time.time - ch.lastGroundedCheckTime >= ch.acd.groundedSwitchCooldown)
			{
				ch.isGrounded = groundedByDistance;
				ch.lastGroundedCheckTime = Time.time;

				//reset jumps on grounded
				if (ch.isGrounded)
				{
					ch.timeSinceLastGrounding = Time.time;

					ch.onGrounding = true;

					ch.ScheduleAction(onGroundingHoldFrames, () => ch.onGrounding = false);
				}
				else
				{
					ch.onUngrounding = true;

					ch.ScheduleAction(onUngroundingHoldFrames, () => ch.onUngrounding = false);
				}
			}
		}
	}
	#endregion grounding
	//=//-----|Routes|--------------------------------------------------//=//
	#region routes
	protected virtual void HandleGrounding()
	{
		if (ch.onGrounding)
		{
			if (!ch.isGroundedBystate)
			{
				StatePushState(CStateID.OO_IdleGrounded, 2, 2);
			}
		}
	}
	protected virtual void HandleJump()
	{
		//assess
		bool jumpAllowed = true;
		if (ch.jumpCount > ch.acd.maxJumps)
		{
			jumpAllowed = false;
		}

		//route 
		if (ih.GetButtonDown("Jump") && jumpAllowed)
		{
			StatePushState(CStateID.OO_Jump, 4, 4);
		}
	}
	#endregion routes
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
	//=//----------------------------------------------------------------//=//
	#endregion level_4
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 5]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_5
	//=//----------------------------------------------------------------//=//
	#endregion level_5
	/////////////////////////////////////////////////////////////////////////////
}
