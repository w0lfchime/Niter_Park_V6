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

    }

    public virtual void Exit()
    {

    }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

}
