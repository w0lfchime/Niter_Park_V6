using System;
using UnityEngine;

public class Home : AppState
{

    public Home()
    {
        AppStateUIScene = "HomeUI";
        AppState3DScene = "Home3D";
    }

    // Lifecycle Methods
    public override void Enter()
    {


    }

    public override void Exit()
    {



        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        // Add Home-specific update logic if needed
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        // Add Home-specific physics updates if needed
    }
}
