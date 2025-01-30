using UnityEngine;

public class PhysicalState : CharacterState
{

    public PhysicalState(Character character) : base(character)
    {

    }



    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        //base.Update();

        DataUpdates();
        ProcessInput();
        Animate();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    public override void ProcessInput()
    {
        base.ProcessInput();
        //access input manager, request input 
    }

    public override void DataUpdates()
    {
        base.DataUpdates();

    }

    public override void Animate()
    {
        base.Animate();

    }

    // - - - - - - - - 


    void ApplyGravity()
    {
        ch.appliedForce += new Vector3(0, -10, 0);
    }

    void ApplyMotion()
    {
        rb.AddForce(ch.appliedForce, ForceMode.Force);
        rb.AddForce(ch.appliedImpulseForce, ForceMode.Impulse);
    }
    void ProcessVerticalMovement()
    {
        //Option 1: handle the jump just as basic movement.
        //Add jump force directly, then maintain jump progression with upward target speed. 
        //cut target speed influence once max jump hold is complete 
        //Option 2: handle the jump by manipualting gravity
        //always apply the base jump force directly.
        //disable or alter gravity during/based on jump hold time

        //Jump count resets on grounded. 

        //Option 2: Gravity Jumping

        Vector3 appliedVerticalForce = Vector3.zero;

        Vector3 appliedVerticalImpluseForce = Vector3.zero;

        //Perform a jump
        if (jumpDown && jumpCount < maxJumps)
        {
            appliedVerticalImpluseForce += Vector3.up; //Vector3.Lerp(Vector3.up, movementInput, jumpDirectionalInfluenceFactor);
            appliedVerticalImpluseForce *= jumpForce;
            lastJumpTime = Time.time;

            rigidBody.velocity = new Vector3(velocityX, 0, 0);
        }

        appliedImpulseForce += appliedVerticalImpluseForce;

        if (jumpHold && Time.time - lastJumpTime < maxJumpHoldTime)
        {
            //immediately ignore gravity
            appliedGravityFactor = gravityFactorWhileJumping;
        }
        else
        {

            appliedGravityFactor = Mathf.Lerp(appliedGravityFactor, gravityFactor, Time.deltaTime * gravityFactorLerpRate);
        }

        targetVelocityY = gravityTerminalVelocity;
        float velocityDiffY = targetVelocityY - velocityY;

        appliedVerticalForce.y += velocityDiffY * appliedGravityFactor;

        appliedForce += appliedVerticalForce;


        //diving mechanic, when veritcal velocity is within a small negative range, allow impulse dropping to the ground. 
    }

    private void CheckGrounded()
    {
        float sphereRadius = cc.radius;
        Vector3 capsuleRaycastStart = ch.transform.position + new Vector3(0, sphereRadius, 0);

        Debug.DrawRay(capsuleRaycastStart, Vector3.down * ch.acd.groundCheckingDistance, Color.red);
        Debug.DrawRay(capsuleRaycastStart + new Vector3(1, 0, 0), Vector3.down * ch.acd.isGroundedDistance, Color.blue);

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
}
