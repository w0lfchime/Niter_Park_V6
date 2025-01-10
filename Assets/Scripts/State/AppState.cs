using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AppState
{
    public abstract string AppStateUIScene { get; }
    public abstract string AppState3DScene { get; }

    // Service locator references
    protected InputManager _inputManager { get; private set; }
    protected UIManager _uiManager { get; private set; }
    protected SettingsManager _settingsManager { get; private set; }

    // SubState management
    private Stack<SubState> _subStateStack = new Stack<SubState>();

    public AppState()
    {
        // Optionally initialize service locators here
    }

    // Lifecycle Methods
    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {
        Debug.Log($"Exiting AppState: {GetType().Name}");
        while (_subStateStack.Count > 0)
        {
            PopSubState();
        }
    }

    public virtual void Update()
    {
        if (_subStateStack.Count > 0)
        {
            _subStateStack.Peek().Update();
        }
    }

    public virtual void FixedUpdate()
    {
        if (_subStateStack.Count > 0)
        {
            _subStateStack.Peek().FixedUpdate();
        }
    }

    // SubState Flow Management
    public void PushSubState(SubState subState)
    {
        if (_subStateStack.Count > 0)
        {
            _subStateStack.Peek().Pause();
        }

        _subStateStack.Push(subState);
        subState.Enter();
    }

    public void PopSubState()
    {
        if (_subStateStack.Count > 0)
        {
            SubState exitingState = _subStateStack.Pop();
            exitingState.Exit();

            if (_subStateStack.Count > 0)
            {
                _subStateStack.Peek().Resume();
            }
        }
    }

    public SubState CurrentSubState => _subStateStack.Count > 0 ? _subStateStack.Peek() : null;
}
