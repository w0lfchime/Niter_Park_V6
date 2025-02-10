using UnityEngine;
using UnityEngine.InputSystem;

public class SuspendedState : PhysicalState 
{

    public SuspendedState(Character character) : base(character)
    {

        exitOnExitAllowed = false;

    }

    public override void Enter()
    {

        base.Enter();

        exitAllowed = true;
    }

    public override void Exit()
    {

        

    }

    public override void Update()
    {


    }

    public override void FixedUpdate()
    {

        

    }

    public override void CheckExitAllowed()
    {
        base.CheckExitAllowed();


    }

    public override void TryRouteState()
    {
        if (exitAllowed)
        {
            if (Input.anyKeyDown)
            {
                ch.TrySetState(defaultExitState, 2);
            }

            base.TryRouteState();
        }
    }
}
