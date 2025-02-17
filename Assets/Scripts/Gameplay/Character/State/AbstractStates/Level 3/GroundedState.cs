using UnityEngine;

public class GroundedState : PhysicalState
{
    //======// /==/==/==/==||[LOCAL FIELDS]||=/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 0]||==/==/==/==/==/==/==/==/==/==/ //======//
    //=//-----|Setup|----------------------------------------------------//=//
    //=//-----|Data Management|------------------------------------------//=//
    //=//-----|Flow Control|---------------------------------------------//=//
    //=//-----|MonoBehavior|---------------------------------------------//=//
    //=//-----|Debug|----------------------------------------------------//=//
    //======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//

    //======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//

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

	public override void FixedFrameUpdate()
	{

		base.FixedFrameUpdate();

	}
}
