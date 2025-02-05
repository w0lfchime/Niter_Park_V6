using UnityEngine;

public class AirborneState : PhysicalState
{
    //in the player can input and move in the air
    public bool allowDrift; 


    public AirborneState(Character character) : base(character)
    {

    }

    public override void Enter()
    {
        allowDrift = true;  
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
        ApplyGravity();
        HandleDrift();
        base.FixedUpdate();

    }
    public virtual void HandleDrift()
    {
        if (allowDrift)
        {
            Vector3 tv = ch.inputMoveDirection;
            tv *= ch.acd.driftMaxSpeed;
            AddForceByTargetVelocity("Drift", tv, ch.acd.driftForceFactor);
        }
    }

    protected virtual void ApplyGravity()
    {
        Vector3 gravForceVector = Vector3.up * ch.acd.gravityTerminalVelocity;
        AddForce("Gravity", gravForceVector);
    }
}
