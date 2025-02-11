using UnityEngine;

public class GroundedState : PhysicalState
{



    public GroundedState(Character character) : base(character)
    {
    }
    public override void Enter()
    {
        base.Enter();

        ch.isGroundedBystate = true;
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

    }
}
