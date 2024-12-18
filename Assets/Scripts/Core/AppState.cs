using System;
using UnityEngine;

public abstract class AppState
{
    protected InputManager _inputManager { get; private set; }
    protected UIManager _uiManager { get; private set; }
    protected SettingsManager _settingsManager { get; private set; }

    public AppState()
    {

    }

    // Lifecycle Methods
    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }**

}
