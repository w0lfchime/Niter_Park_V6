using UnityEngine;

public class AirborneState : PhysicalState
{
    //in the player can input and move in the air
    public bool allowDrift; 


    public AirborneState(Character character) : base(character)
    {

    }
    public override void SetStateReferences()
    {
        base.SetStateReferences();


    }

    /// <inheritdoc/>
    /// <summary>
    /// meow foonk shawpoi
    /// </summary>
    public override void SetStateParameters()
    {
        base.SetStateParameters();


    }
    public override void SetStateVariablesOnEntry()
    {
        base.SetStateVariablesOnEntry();


    }

    public override void Enter()
    {
        base.Enter();

        allowDrift = true;  

        ch.isGroundedBystate = false;
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

    /// <summary>
    /// Use ground checking data to prepar for proper grounding. Accelerate animations, prepare state routing.
    /// Grounding may not even occur. 
    /// </summary>
    protected virtual void WatchGrounding()
    {

    }
}
