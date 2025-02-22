using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class SystemInputManager
{
	private AppManager appManager;
	private List<InputDevice> availableDevices = new List<InputDevice>();


	public bool localPlayerCanJoin = true;



	public SystemInputManager(AppManager manager)
	{
		appManager = manager;

		// Listen for new device connections
		InputSystem.onDeviceChange += OnDeviceChange;

		// Listen for input from unassigned devices
		InputSystem.onAnyButtonPress.Call(OnUnpairedDeviceUsed);
	}

	~SystemInputManager()
	{
		// Cleanup to prevent memory leaks
		InputSystem.onDeviceChange -= OnDeviceChange;
	}

	private void OnDeviceChange(InputDevice device, InputDeviceChange change)
	{
		if (change == InputDeviceChange.Added)
		{
			if (!availableDevices.Contains(device))
			{
				availableDevices.Add(device);
				Debug.Log($"Device added: {device.displayName}");
			}
		}
		else if (change == InputDeviceChange.Removed)
		{
			availableDevices.Remove(device);
			//TODO: manage player 
			Debug.Log($"Device removed: {device.displayName}");
		}
	}

	private void OnUnpairedDeviceUsed(InputControl control)
	{
		if (localPlayerCanJoin)
		{
			InputDevice device = control.device;

			if (availableDevices.Contains(device))
			{
				Debug.Log($"Device used: {device.displayName}");

				//availableDevices.Remove(device);
			}
		}

	}


}
