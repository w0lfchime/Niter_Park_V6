using UnityEngine;

public class IdleGroundedState : GroundedState
{

    //IdleGrounded will frequently be a routing state.
    //This means that states will route to idle grounded concerened only with grounding the character.
    //IdleGrounded will handle routing to the specific state on its own.
    //It has no actions of its own, thus is does not require a non-zero min duration. .
    public IdleGroundedState(Character character) : base(character)
    {
        exitOnPriorityZero = false;
        minimumStateDuration = 0.0f;
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

    public override void FixedFrameUpdate()
    {

        base.FixedFrameUpdate();

    }
}
