using System;
using System.Security.Cryptography;
using UnityEngine;

public class PhysicalState : CharacterState
{
	//Level 2 state 
	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	private int? onGroundingHoldFrames = 5;
	private int? onUngroundingHoldFrames = 5;
	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
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
	public override void SetStateMembers()
	{
		base.SetStateMembers();
		//...
	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected override void SetOnEntry()
	{
		base.SetOnEntry();
		//...
		PhysicalDataUpdates(); //call here to prevent nulls or whatever
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
		HandleNaturalRotation();
	}
	public override void FixedFrameUpdate()
	{
		SetGrounding();
		//...
		base.FixedFrameUpdate();
	}
	public override void FixedPhysicsUpdate()
	{
		WatchGrounding();
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
		Vector3 forceByTargetVelocity = Vector3.zero;
		forceByTargetVelocity += targetVelocity - ch.velocity;
		forceByTargetVelocity *= forceFactor;
		AddForce(forceName, forceByTargetVelocity);
	}
	protected virtual void ApplyGravity()
	{
		Vector3 gravForceVector = Vector3.up * ch.acs.gravityTerminalVelocity;
		AddForce("Gravity", gravForceVector);
	}
	#endregion force
	//=//-----|Grounding|-----------------------------------------------//=//
	#region grounding
	protected virtual void WatchGrounding()
	{
		float sphereRadius = cc.radius;
		Vector3 capsuleRaycastStart = ch.transform.position + new Vector3(0, sphereRadius + 0.1f, 0);

		UnityEngine.Debug.DrawRay(capsuleRaycastStart, Vector3.down * ch.acs.groundCheckingDistance, Color.red);
		UnityEngine.Debug.DrawRay(capsuleRaycastStart + new Vector3(0.1f, 0, 0), Vector3.down * ch.acs.isGroundedDistance, Color.blue);

		RaycastHit hit;

		if (Physics.SphereCast(capsuleRaycastStart, sphereRadius, Vector3.down, out hit, ch.acs.groundCheckingDistance, ch.groundLayer))
		{
			ch.distanceToGround = hit.distance - sphereRadius;
		}
		else
		{
			ch.distanceToGround = ch.acs.groundCheckingDistance;
		}


	}
	public void SetGrounding()
	{
		bool groundedByDistance = ch.distanceToGround < ch.acs.isGroundedDistance;

		if (groundedByDistance != ch.isGrounded)
		{
			if (Time.time - ch.lastGroundedCheckTime >= ch.acs.groundedSwitchCooldown)
			{
				ch.isGrounded = groundedByDistance;
				ch.lastGroundedCheckTime = Time.time;

				//reset jumps on grounded
				if (ch.isGrounded)
				{
					ch.timeSinceLastGrounding = Time.time;

					ch.onGrounding = true;

					ch.ScheduleAction((int)onGroundingHoldFrames, () => ch.onGrounding = false);
				}
				else
				{
					ch.onUngrounding = true;

					ch.ScheduleAction((int)onUngroundingHoldFrames, () => ch.onUngrounding = false);
				}
			}
		}
	}
	#endregion grounding
	//=//-----|Rotation|------------------------------------------------//=//
	#region rotation
	public void HandleNaturalRotation()
	{
		ch.facingRight = ch.velocityX > 0;

		ch.clockwiseRotation = ch.facingRight;

		Vector3 directionFacing = ch.facingRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);

		// Calculate the target rotation on the y-axis
		Quaternion targetRotation = Quaternion.LookRotation(directionFacing, Vector3.up);

		// Get the current rotation as Euler angles
		Vector3 currentEuler = ch.rigAndMeshTransform.eulerAngles;

		// Get the target rotation as Euler angles
		Vector3 targetEuler = targetRotation.eulerAngles;

		// Calculate the shortest direction to rotate on the y-axis
		float currentY = currentEuler.y;
		float targetY = targetEuler.y;

		if (ch.clockwiseRotation)
		{
			if (currentY < targetY)
				currentY += 360; // Prevent rotating the long way around
		}
		else
		{
			if (currentY > targetY)
				targetY += 360; // Prevent rotating the long way around
		}

		// Smoothly lerp the y-axis rotation
		float newY = Mathf.Lerp(currentY, targetY, Time.deltaTime * ch.acs.rotationSpeed);

		// Set the new rotation while preserving other axes
		ch.rigAndMeshTransform.rotation = Quaternion.Euler(currentEuler.x, newY, currentEuler.z);
	}

	#endregion rotation
	//=//-----|Routes|--------------------------------------------------//=//
	#region routes
	protected virtual void HandleGrounding()
	{
		if (ch.onGrounding)
		{
			if (!ch.isGroundedByState)
			{
				StatePushState(CStateID.OO_IdleGrounded, (int)priority + 1, 2);
			}
		}
		if (ch.onUngrounding)
		{
			if (ch.isGroundedByState)
			{
				StatePushState(CStateID.OO_IdleAirborne, (int)priority + 1, 2);
			}
		}
	}
	protected virtual void HandleJump()
	{
		//assess
		bool jumpAllowed = true;
		if (ch.jumpCount > ch.acs.maxJumps)
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
}
