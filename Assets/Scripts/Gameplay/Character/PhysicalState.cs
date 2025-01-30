using UnityEngine;

public class PhysicalState : CharacterState
{

    public PhysicalState(Character character) : base(character)
    {

    }



    public override void Entry()
    {
        base.Entry();

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
        appliedForce += new Vector3(0, -10, 0);
    }

    void ApplyMotion()
    {
        rb.AddForce(appliedForce, ForceMode.Force);
        rb.AddForce(appliedImpulseForce, ForceMode.Impulse);
    }


    private void CheckGrounded()
    {
        float sphereRadius = capsuleCollider.radius;
        Vector3 capsuleRaycastStart = transform.position + new Vector3(0, sphereRadius, 0);

        Debug.DrawRay(capsuleRaycastStart, Vector3.down * groundCheckingDistance, Color.red);
        Debug.DrawRay(capsuleRaycastStart + new Vector3(1, 0, 0), Vector3.down * isGroundedDistance, Color.blue);

        RaycastHit hit;

        if (Physics.SphereCast(capsuleRaycastStart, sphereRadius, Vector3.down, out hit, groundCheckingDistance, groundLayer))
        {
            distanceToGround = hit.distance - sphereRadius;
        }
        else
        {
            distanceToGround = groundCheckingDistance;
        }

        bool newGroundedState = distanceToGround < isGroundedDistance;

        onGrounding = false;
        onUngrounding = false;

        if (Time.time - lastGroundedCheckTime >= groundedSwitchCooldown && newGroundedState != isGrounded)
        {
            isGrounded = newGroundedState;
            lastGroundedCheckTime = Time.time;

            //reset jumps on grounded
            if (isGrounded)
            {
                jumpCount = 0;
                timeSinceLastGrounding = Time.time;

                onGrounding = true;

            }
            else
            {
                onUngrounding = true;
            }
        }
    }
}
