using UnityEngine;
using UnityEngine.InputSystem;

public class SuspendedState : PhysicalState 
{

    public SuspendedState(Character character) : base(character)
    {

        exitOnExitAllowed = true;
    }

    public override void Enter()
    {

        base.Enter();

    }

    public override void Exit()
    {

        

    }

    public override void Update()
    {
        if (exitAllowed)
        {
            if (Input.anyKeyDown)
            {
                string newState = "IdleAirborne";
                if (ch.isGrounded)
                {
                    newState = "IdleGrounded";
                }
                ch.TrySetState(newState, 0);
            }

        }


    }

    public override void FixedUpdate()
    {

        

    }

    public override void CheckExitAllowed()
    {
        base.CheckExitAllowed();


    }

    public override void 
}
