using UnityEngine;

public class AirborneState : PhysicalState
{
    //======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
    //=//-----|Parameters|------------------------------------------------//=//


    //=//-----|Variables|-------------------------------------------------//=//
    [Header("Drift")]
    public bool allowDrift;


    //======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
    //=//-----|Setup|----------------------------------------------------//=//
    public AirborneState(Character character) : base(character)
    {

    }
    protected override void SetStateReferences()
    {
        base.SetStateReferences();
        //...
    }
    protected override void SetStateParameters()
    {
        base.SetStateParameters();
        //...
    }
    //=//-----|Data Management|------------------------------------------//=//
    protected override void SetOnEntry()
    {
        base.SetOnEntry();
        //...
        allowDrift = true;
    }
    //=//-----|Flow Control|---------------------------------------------//=//
    public override void Enter()
    {
        base.Enter();
        //... 

    }
    public override void Exit()
    {
        base.Exit();
        //...
    }
    protected override void CheckExitAllowed()
    {
        base.CheckExitAllowed();
        //...
    }
    protected override void TryRouteState()
    {
        //...
        base.TryRouteState();
    }
    protected override void TryRouteStateFixed()
    {
        //...
        base.TryRouteStateFixed();
    }

    //=//-----|MonoBehavior|---------------------------------------------//=//
    public override void Update()
    {
        //...
        base.Update();
    }
    public override void FixedFrameUpdate()
    {
        ApplyGravity();
        HandleDrift();

        //...
        base.FixedFrameUpdate();
    }
    public override void LateUpdate()
    {
        //...
        base.LateUpdate();
    }
    //=//-----|Debug|----------------------------------------------------//=//
    public override bool VerifyState()
    {
        return base.VerifyState();
    }

    //======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======//
    //Level 1, Physical State
    //=//-----|Force|--------------------------------------------------//=//
    protected override void ApplyGravity()
    {
        base.ApplyGravity(); //? different implementation?
    }
    //======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//
    //Level 2, Airborne State (this)
    //=//-----|Local Actions|---------------------------------------//=//
    public virtual void HandleDrift()
    {
        if (allowDrift)
        {
            Vector3 tv = ch.inputMoveDirection;
            tv *= ch.acd.driftMaxSpeed;
            AddForceByTargetVelocity("Drift", tv, ch.acd.driftForceFactor);
        }
    }
    //=//-----|Routes|----------------------------------------------//=//
    //some sort of function for attacks, 

    //======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
}
