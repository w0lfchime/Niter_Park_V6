using UnityEngine;

public class Suspended : CharacterState
{
	public Suspended(PerformanceSM sm, Character character) : base(sm, character)
	{
		minimumStateDuration = 120;
	}
	public override void Enter()
	{
		base.Enter();

		//...

		LogCore.Log("StateWarning", "Entering the Suspended CState. Something wrong probably happened");
	}

	public override void Exit()
	{
		base.Exit();

		//...
	}

	public override void Update()
	{
		base.Update();

		//...
	}

	public override void FixedFrameUpdate()
	{
		base.FixedFrameUpdate(); 

		//...

	}

	public override void FixedPhysicsUpdate()
	{
		base.FixedPhysicsUpdate();

		//...

	}

	public override void LateUpdate()
	{
		base.LateUpdate();

		//...
	}


}
