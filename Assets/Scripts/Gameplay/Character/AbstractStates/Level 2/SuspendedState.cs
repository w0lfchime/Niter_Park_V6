using UnityEngine;
using UnityEngine.InputSystem;

public class SuspendedState : PhysicalState 
{
    public bool canExit = true;

    public SuspendedState(Character character) : base(character)
    {
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
        if (canExit)
        {
            if (Input.anyKeyDown)
            {
                string newState = "Airborne";
                if (ch.isGrounded)
                {
                    newState = "Grounded";
                }
                ch.TrySetState(newState, 0);
            }

        }


    }

    public override void FixedUpdate()
    {

        

    }
}
