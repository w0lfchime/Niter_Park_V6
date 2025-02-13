using UnityEngine;

public class ExampleState : CharacterState
{
    //======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
    //=//-----|Setup|----------------------------------------------------//=//
    public ExampleState(Character character) : base(character)
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
        //...D
    }
    //=//-----|Data Management|------------------------------------------//=//
    protected override void SetVariablesOnEntry()
    {
        base.SetVariablesOnEntry();
        //...
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
    public override void FixedUpdate()
    {
        //...
        base.FixedUpdate();
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

    //======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
}
