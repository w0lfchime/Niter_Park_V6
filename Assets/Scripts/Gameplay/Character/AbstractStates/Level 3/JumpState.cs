using UnityEngine;

public class JumpState : AirborneState
{
    public JumpState(Character character) : base(character)
    {
    }

    public override void Enter()
    {
        base.Enter();

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

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (ch.velocityY < 0)
        {
            ch.SetState("IdleAirborne");
        }



    }
}
