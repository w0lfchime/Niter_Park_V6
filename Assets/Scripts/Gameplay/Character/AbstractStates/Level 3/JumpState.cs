using UnityEngine;

public class JumpState : AirborneState
{
    private float minimumExitVelocityY = -0.2f;

    //Exit allowed is true by default.
    public JumpState(Character character) : base(character)
    {
        minimumStateDuration = 0.2f;
        exitOnPriorityZero = true;
        exitState = "IdleAirborne";
    }

    public override void Enter()
    {
        base.Enter();
        
        //clearing vertical velocity 
        Vector3 newVelocity = ch.rigidBody.linearVelocity;
        newVelocity.y = 0;
        ch.rigidBody.linearVelocity = newVelocity;

        Vector3 jumpImpluseForce = Vector3.up;
        jumpImpluseForce *= ch.acd.jumpForce;

        AddImpulseForce("JumpForce", jumpImpluseForce);

    }

    public override void Exit()
    {

        base.Exit();

    }

    public override void Update()
    {


        base.Update();

    }



    public override void FixedFrameUpdate()
    {
        base.FixedFrameUpdate();




    }

    public override void CheckExitAllowed()
    {
        if (ch.velocityY < minimumExitVelocityY)
        {
            exitAllowed = true;
        }

        base.CheckExitAllowed();


    }

    public override void TryRouteState()
    {



        base.TryRouteState();

    }
}
