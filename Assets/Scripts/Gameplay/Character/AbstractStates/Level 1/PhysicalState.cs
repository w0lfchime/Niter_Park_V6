using UnityEngine;

public class PhysicalState : CharacterState
{
    //=//-----|Static Parameters|-----------------------------------------//=//
    public static int onGroundingHoldFrames = 2;
    public static int onUngroundingHoldFrames = 2;

    //======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
    //=//-----|Parameters|------------------------------------------------//=//


    //=//-----|Variables|-------------------------------------------------//=//


    //======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
    //=//-----|Setup|----------------------------------------------------//=//
    public PhysicalState(Character character) : base(character)
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
        //...
    }
    //=//-----|Data Management|------------------------------------------//=//
    protected override void SetOnEntry()
    {
        base.SetOnEntry();
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
        //...

        HandleGrounding();
        HandleJump();

        base.TryRouteState();
    }

    protected override void TryRouteStateFixed()
    {


        base.TryRouteStateFixed();
    }

    //=//-----|MonoBehavior|---------------------------------------------//=//
    public override void Update()
    {
        PhysicalDataUpdates();

        //...
        base.Update();
    }
    public override void FixedFrameUpdate()
    {
        WatchGrounding();
        SetGrounding();

        //...
        base.FixedFrameUpdate();
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
    //=//-----|Data Management|-----------------------------------------//=//
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
    //=//-----|Force|---------------------------------------------------//=//
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
    //=//-----|Grounding|-----------------------------------------------//=//
    /// <summary>
    /// observes the distance from the ground
    /// </summary>
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
    /// <summary>
    /// uses information from WatchGrounding() to set grounding variables
    /// </summary>
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

    //=//-----|State Routes|--------------------------------------------//=//
    protected virtual void HandleJump()
    {
        if (pinput.GetButtonDown("Jump"))
        {
            if (!ch.onGrounding)
            {


            }
        }



        if (ch.jumpCount >= ch.acd.maxJumps) 
        {
            jumpAllowed = false;
        }
        

        if ()
        {
            ch.PushState("Jump", 4);
        }
    }
    protected virtual void HandleGrounding()
    {
        if (ch.onGrounding)
        {
            if (!ch.isGroundedBystate)
            {
                ch.PushState("IdleGrounded", 2);
            }
        }
    }


    //======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
}
