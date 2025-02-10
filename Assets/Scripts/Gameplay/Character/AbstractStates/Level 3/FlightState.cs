using UnityEngine;

public class FlightState : AirborneState
{
    //The flight state is basically a suspended airborne state, so the character class can directly
    //register the flight state. TODO: implement as such
    public FlightState(Character character) : base(character)
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

        base.Update();

    }

    public override void FixedUpdate()
    {

        base.FixedUpdate();

    }

}
