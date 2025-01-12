using UnityEngine;

public class HomeMenuHome : SubState
{
    public HomeMenuHome(AppState parentState) : base(parentState)
    {
    }

    public override void Enter()
    {
        base.Enter();

        LogCore.Log("Entered HomeMenuHome SubState", LogCategory.SubState);

        // Example: Display the main menu UI
        UIManager.Instance.ShowMenu("MainMenu");
    }

    public override void Exit()
    {
        base.Exit();



    }

    public override void Update()
    {
        base.Update();

        HandleInput();
    }

    public override void Pause()
    {
        base.Pause();

        LogCore.Log("Paused HomeMenuHome SubState", LogCategory.SubState);
    }

    public override void Resume()
    {
        base.Resume();

        LogCore.Log("Resumed HomeMenuHome SubState", LogCategory.SubState);
    }
}
