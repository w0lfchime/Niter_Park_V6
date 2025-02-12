using UnityEngine;

public class PhysicalState22 : CharacterState
{
    //======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
    //=//-----|Setup|----------------------------------------------------//=//
    public PhysicalState22(Character character) : base(character)
    {
        //nothing yet
    }
    protected override void SetStateReferences()
    {
        base.SetStateReferences();
        //...
    }
    protected override void SetStateParameters()
    {
        base.SetStateParameters();
        //...D
    }
    //=//-----|Data Management|------------------------------------------//=//
    protected override void SetStateVariablesOnEntry()
    {
        base.SetStateVariablesOnEntry();
        //...
    }
    //=//-----|Flow Control|---------------------------------------------//=//
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
    protected override void CheckExitAllowed()
    {
         base.CheckExitAllowed();
        //...
    }
    protected override void TryRouteState()
    {
        //something, idk what yet

        base.TryRouteState();
    }

    protected override void TryRouteStateFixed()
    {
        //this only processes grounding in an instant 
        if (ch.onGrounding)
        {
            if (!ch.isGroundedBystate)
            {
                ch.TrySetState("IdleGrounded", 2);
            }
        }

        base.TryRouteStateFixed();
    }

    //=//-----|MonoBehavior|---------------------------------------------//=//
    public override void Update()
    {
        //...
        base.Update();
    }
    public override void FixedUpdate()
    {
        //...
        base.FixedUpdate();
    }
    public override void LateUpdate()
    {
        //...
        base.LateUpdate();
    }
    //=//-----|Debug|----------------------------------------------------//=//
    public override bool VerifyState()
    {
        return base.VerifyState();
    }

    //======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======//
    //=//-----|Data Management|------------------------------------------//=//
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
    //=//-----|Forces|--------------------------------------------------//=//
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

    //=//-----|Physical Action|------------------------------------------//=//
    protected virtual void HandleJump()
    {
        if (ch.jumpAllowedByContext && pinput.GetButtonDown("Jump"))
        {
            ch.TrySetState("Jump", 4);
        }
    }
    //=//-----|Grounding|-----------------------------------------------//=//
    protected virtual void CheckGrounded()
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

    //======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
}
