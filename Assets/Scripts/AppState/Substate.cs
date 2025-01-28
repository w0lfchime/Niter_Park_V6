using UnityEngine;

public abstract class SubState
{
    protected AppState _appState { get; private set; }

    public SubState(AppState parentState)
    {
        _appState = parentState;
    }

    public virtual void Enter()
    {
        LogCore.Log("Entering SubState: {this.GetType().Name}", LogCategory.SubState);
    }

    public virtual void Exit()
    {
        LogCore.Log("Exiting SubState: {this.GetType().Name}", LogCategory.SubState);
    }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void Pause() { }

    public virtual void Resume() { }

}
