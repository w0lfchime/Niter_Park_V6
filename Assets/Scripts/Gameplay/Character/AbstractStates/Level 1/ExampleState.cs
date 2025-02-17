using System;
using UnityEngine;

public class ExampleState : CharacterState
{
	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////





	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public ExampleState(PerformanceSM sm, Character character) : base(sm, character)
    {

    }
    protected override void SetStateReferences()
    {
        base.SetStateReferences();
        //...
    }
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected override void SetOnEntry()
    {
        base.SetOnEntry();
        //...
    }
	protected override void PerFrame()
	{
		base.PerFrame();
		//...
	}
	#endregion data_management
	//=//-----|Flow|-----------------------------------------------------//=//
	#region flow
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
	#endregion flow
	//=//-----|Mono|-----------------------------------------------------//=//
	#region mono
	public override void Update()
    {
        //...
        base.Update();
    }
    public override void FixedFrameUpdate()
    {
        //...
        base.FixedFrameUpdate();
    }
	public override void FixedPhysicsUpdate()
	{
		//...
		base.FixedPhysicsUpdate();
	}
	public override void LateUpdate()
    {
        //...
        base.LateUpdate();
    }
	#endregion mono
	//=//-----|Routing|--------------------------------------------------//=//
	#region routing
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
	#endregion routing
	//=//-----|Debug|----------------------------------------------------//=//
	#region debug
	public override bool VerifyState()
    {
        return base.VerifyState();
    }
	#endregion debug
	//=//----------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======//
	#region level_1
	//=//----------------------------------------------------------------//=//
	#endregion level_1
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//

	//======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//

	//======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
}
