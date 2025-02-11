using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler
{
    public PlayerInput playerInput;
    public Dictionary<string, bool> buttonDown;
    public Dictionary<string, bool> buttonHold;
    public Dictionary<string, bool> buttonUp;

    public bool anyActionAtAll = false;
    public PlayerInputHandler(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        buttonDown = new Dictionary<string, bool>();
        buttonHold = new Dictionary<string, bool>();
        buttonUp = new Dictionary<string, bool>();

        foreach (var action in playerInput.actions)
        {
            buttonDown[action.name] = false;
            buttonHold[action.name] = false;
            buttonUp[action.name] = false;
        }
    }

    public void UpdateInputs()
    {
        anyActionAtAll = false;
        foreach (var action in playerInput.actions)
        {
            buttonDown[action.name] = action.WasPressedThisFrame();
            buttonUp[action.name] = action.WasReleasedThisFrame();

            bool pressed = action.IsPressed();
            buttonHold[action.name] = pressed;
            if (pressed)
            {
                anyActionAtAll = true;
            }
        }
    }

    public bool GetButtonDown(string actionName) => buttonDown.TryGetValue(actionName, out bool value) && value;
    public bool GetButtonHold(string actionName) => buttonHold.TryGetValue(actionName, out bool value) && value;
    public bool GetButtonUp(string actionName) => buttonUp.TryGetValue(actionName, out bool value) && value;
}
