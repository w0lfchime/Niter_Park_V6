using UnityEngine;

public class AirborneState : PhysicalState
{


    public AirborneState(Character character) : base(character)
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
        CheckGrounded();

        base.Update();

    }

    public override void FixedUpdate()
    {
        ApplyGravity();

        base.FixedUpdate();

    }

    protected virtual void ApplyGravity ()
    {
        ch.appliedForce += Vector3.down * 5;
    }
}
