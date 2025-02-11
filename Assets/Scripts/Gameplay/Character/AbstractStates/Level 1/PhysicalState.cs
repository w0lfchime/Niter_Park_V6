using System.Diagnostics;
using System.Xml.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PhysicalState : CharacterState
{
	public PhysicalState(Character character) : base(character)
	{
		//HACK: set minimum state duration to small value to prevent fluttering. Some
		//states will probably override this.
		minimumStateDuration = 0.2f;
		exitOnExitAllowed = false; //redundant 
	}

	//public StateName(Character character) : base (character)
	//{

	//}
    public override void SetReferences()
    {
        base.SetReferences();


    }
    public override void SetStateParameters()
    {
        base.SetStateParameters();


    }
	public override void SetStateVariablesOnEntrance()
	{
		base.SetStateVariablesOnEntrance();


	}


    public override void Enter()
	{
		base.Enter();

	}

	public override void Exit()
	{
		base.Exit();


		//clear physics values?
		//save physics values?
	}

	public override void Update()
	{
		base.Update();

		PhysicalDataUpdates();

		HandleJump();

	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();

        CheckGrounded();
        ApplyImpulseForces();
		ApplyForces();

		TryRouteStateFixed();
	}

	void PhysicalDataUpdates()
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

	void HandleJump()
	{
		if (ch.jumpAllowedByContext && pinput.GetButtonDown("Jump")) {
			ch.TrySetState("Jump", 4);
		}
	}

	//Handling Forces
	public virtual void AddForceByTargetVelocity(string forceName, Vector3 targetVelocity, float forceFactor)
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

	//Force Management - - - - - - - - - - - - -
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


	protected void ApplyForces()
	{
		ch.UpdateDebugVector("AppliedForceVector", ch.appliedForce, Color.blue);
		rb.AddForce(ch.appliedForce, ForceMode.Force);
		ch.appliedForce = Vector3.zero;
	}
	protected void ApplyImpulseForces()
	{
		rb.AddForce(ch.appliedImpulseForce, ForceMode.Impulse);
		ch.appliedImpulseForce = Vector3.zero;
	}
	// - - - - - - - - - - - - - - - - - - - - -


	protected void CheckGrounded()
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

		bool newGroundedState = ch.distanceToGround < ch.acd.isGroundedDistance;

		ch.onGrounding = false;
		ch.onUngrounding = false;

		if (Time.time - ch.lastGroundedCheckTime >= ch.acd.groundedSwitchCooldown && newGroundedState != ch.isGrounded)
		{
			ch.isGrounded = newGroundedState;
			ch.lastGroundedCheckTime = Time.time;

			//reset jumps on grounded
			if (ch.isGrounded)
			{
				ch.jumpCount = 0;
				ch.timeSinceLastGrounding = Time.time;

				ch.onGrounding = true;

			}
			else
			{
				ch.onUngrounding = true;
			}
		}
	}

    //State flow - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    public override void TryRouteState()
    {
		//something, idk what yet

        base.TryRouteState();
    }

    public override void TryRouteStateFixed()
    {
		//this only processes grounding in an instant 
		if (ch.onGrounding)
		{
			if(!ch.isGroundedBystate)
			{
				ch.TrySetState("IdleGrounded", 2);
			}
		}

        base.TryRouteStateFixed();
    }


}
