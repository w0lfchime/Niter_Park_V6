using UnityEngine;

public class PhysicalState : CharacterState
{
    PlayerInputHandler ih;
    public PhysicalState(Character character) : base(character)
    {
        this.ih = ch.inputHandler;
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
        CheckGrounded();
        base.Update();

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();


        ApplyImpulseForces();
        ApplyForces();
    }

    void PhysicalDataUpdates()
    {
        Vector3 lv = rb.linearVelocity;

        ch.velocityX = lv.x;
        ch.velocityY = lv.y;
        ch.playerSpeed = lv.magnitude;

    }




    protected void ApplyForces()
    {
        rb.AddForce(ch.appliedForce, ForceMode.Force);
        ch.appliedForce = Vector3.zero;
    }
    protected void ApplyImpulseForces()
    {
        rb.AddForce(ch.appliedImpulseForce, ForceMode.Impulse);
        ch.appliedImpulseForce = Vector3.zero;
    }

    protected void CheckGrounded()
    {
        float sphereRadius = cc.radius;
        Vector3 capsuleRaycastStart = ch.transform.position + new Vector3(0, sphereRadius, 0);

        UnityEngine.Debug.DrawRay(capsuleRaycastStart, Vector3.down * ch.acd.groundCheckingDistance, Color.red);
        UnityEngine.Debug.DrawRay(capsuleRaycastStart + new Vector3(1, 0, 0), Vector3.down * ch.acd.isGroundedDistance, Color.blue);

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
