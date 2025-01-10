using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeAppState : AppState
{

    public override string AppStateUIScene => "HomeMenuUI";
    public override string AppState3DScene => "HomeMenu3D";


    // Constructor that passes the UIManager to the base AppState class
    public HomeAppState()
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
        // Handle specific fixed update logic for the HomeMenu
    }
}
