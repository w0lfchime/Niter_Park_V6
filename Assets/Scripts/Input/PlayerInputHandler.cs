using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
	public PlayerInput playerInput { get; private set; }
	private InputDevice assignedDevice;

	private Dictionary<string, bool> buttonDown = new();
	private Dictionary<string, bool> buttonHold = new();
	private Dictionary<string, bool> buttonUp = new();

	public bool anyActionAtAll { get; private set; }

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		InitializeDictionaries();
	}

	private void InitializeDictionaries()
	{
		foreach (var action in playerInput.actions)
		{
			buttonDown[action.name] = false;
			buttonHold[action.name] = false;
			buttonUp[action.name] = false;
		}
	}

	private void Update()
	{
		if (assignedDevice == null)
		{
			AssignFirstAvailableDevice();
			return;
		}

		anyActionAtAll = false;
		foreach (var action in playerInput.actions)
		{
			bool pressed = action.IsPressed();
			buttonDown[action.name] = action.WasPressedThisFrame();
			buttonUp[action.name] = action.WasReleasedThisFrame();
			buttonHold[action.name] = pressed;

			if (pressed) anyActionAtAll = true;
		}
	}

	private void AssignFirstAvailableDevice()
	{
		if (Gamepad.all.Count > 0)
		{
			AssignDevice(Gamepad.all[0]); // Auto-assign first detected gamepad
		}
	}

	public void AssignDevice(InputDevice device)
	{
		if (device == null) return;

		assignedDevice = device;
		playerInput.SwitchCurrentControlScheme(device);
	}

	public bool GetButtonDown(string actionName) => buttonDown.TryGetValue(actionName, out bool value) && value;
	public bool GetButtonHold(string actionName) => buttonHold.TryGetValue(actionName, out bool value) && value;
	public bool GetButtonUp(string actionName) => buttonUp.TryGetValue(actionName, out bool value) && value;
}
