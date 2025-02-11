using UnityEngine;

public class Suspended : CharacterState
{
	public Suspended(Character character) : base(character)
	{
		exitOnExitAllowed = true;
		minimumStateDuration = 0.2f; 
	}
	public override void Enter()
	{
		base.Enter(); //as always, exitAllowed is set to false.
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

    public override void CheckExitAllowed()
    {
		if (pinput.anyActionAtAll)
		{
			exitAllowed = true; //overridden by min state duration below
		}

        base.CheckExitAllowed(); //exit time is a dominant factor in allowing state acesss
    }

    public override void TryRouteState()
    {
        base.TryRouteState();
    }
}
