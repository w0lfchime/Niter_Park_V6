using System.Diagnostics;
using System.Xml.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting.FullSerializer;
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

        //ch.vrm.ResetVectors();

        //clear physics values?
        //save physics values?
    }

    public override void Update()
    {
        PhysicalDataUpdates();
        CheckGrounded();
        HandleJump();
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
        ch.position = ch.transform.position;

        Vector3 lv = rb.linearVelocity;
        ch.velocity = lv;
        ch.velocityX = lv.x;
        ch.velocityY = lv.y;
        ch.playerSpeed = lv.magnitude;


        //debug 
        ch.vrm.UpdateVector(GenerateVectorName("Velocity"), ch.transform, lv, Color.green);

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
        ch.vrm.UpdateVector(GenerateVectorName(tvName), ch.transform, targetVelocity, Color.white);

        //force
        Vector3 forceByTargetVeloity = Vector3.zero;
        forceByTargetVeloity += targetVelocity - ch.velocity;
        forceByTargetVeloity *= forceFactor;
        AddForce(forceName, forceByTargetVeloity);
    }

    public virtual void AddForce(string forceName, Vector3 force)
    {
        if (ch.debug)
        {
            ch.vrm.UpdateVector(GenerateVectorName(forceName), ch.transform, force, Color.yellow);
        }

        ch.appliedForce += force;

    }
    public virtual void AddImpulseForce(string forceName, Vector3 impulseForce)
    {
        if (ch.debug)
        {

            ch.vrm.StampVector(GenerateVectorName(forceName), ch.transform.position, impulseForce, Color.red, 1.0f);
        }

        ch.appliedImpulseForce += impulseForce;
    }

    public virtual string GenerateVectorName(string vectorName)
    {
        return $"{ch.name}_{vectorName}";
    }

    protected void ApplyForces()
    {
        ch.vrm.UpdateVector(GenerateVectorName("AppliedForceVector"), ch.transform, ch.appliedForce, Color.blue);
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
