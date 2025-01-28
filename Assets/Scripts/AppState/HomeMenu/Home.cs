using System;
using UnityEngine;

public class Home : AppState
{
    public override string AppStateUIScene => "HomeUI"; // Replace with the name of your UI scene
    public override string AppState3DScene => "Home3D"; // Replace with the name of your 3D scene

    public Home()
    {
        // Constructor logic (if needed)
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
