using UnityEngine;

public class GenericFlight : AirborneState
{
    public GenericFlight(Character character) : base(character)
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

        base.Update();

    }

    public override void FixedUpdate()
    {

        base.FixedUpdate();

    }

    protected override void ApplyGravity()
    {
        //no gravity
    }
}
