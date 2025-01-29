using UnityEngine;
using System.Collections.Generic;
using System.Reflection;


public abstract class Character : MonoBehaviour
{
	//giving character class service access?

	//Character Stats and Data
	public CharacterData ucd; //Universal
    public CharacterData bcd; //Base Character Specific
    public CharacterData acd; //Active Universal+Base+Context 

	private CharacterState currentState;
	private Dictionary<string, CharacterState> states = new Dictionary<string, CharacterState>();

	public Rigidbody rigidBody;
	public CapsuleCollider capsuleCollider;
	public Animator animator;

	public void UpdateActiveCharacterData()
	{
		if (ucd == null || bcd == null || acd == null)
		{
			Debug.LogError("Error: ucd, bcd, or acd null.");
			return;
		}

		// Get all fields of the ScriptableObject type
		FieldInfo[] fields = typeof(CharacterData).GetFields(BindingFlags.Public | BindingFlags.Instance);

		foreach (var field in fields)
		{
			// Get the values from the first two objects
			object value1 = field.GetValue(ucd);
			object value2 = field.GetValue(bcd);

			// Handle addition for specific types
			if (value1 is int intValue1 && value2 is int intValue2)
			{
				field.SetValue(acd, intValue1 + intValue2);
			}
			else if (value1 is float floatValue1 && value2 is float floatValue2)
			{
				field.SetValue(acd, floatValue1 + floatValue2);
			}
			else if (value1 is Vector3 vectorValue1 && value2 is Vector3 vectorValue2)
			{
				field.SetValue(acd, vectorValue1 + vectorValue2);
			}
		}
	}

	public virtual void CharacterInitialization()
	{
		rigidBody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		animator = GetComponent<Animator>();

		UpdateActiveCharacterData();

		//define states in child 
	}

	public void BasicEntry()
	{
		CharacterInitialization();
	}

	public void SetState(CharacterState newState)
	{
		if (states.ContainsValue(newState))

			if (currentState != null)
			{
				currentState.Exit();
			}

		currentState = newState;
		if (currentState != null)
		{
			currentState.Enter();
		}
	}

	private void Update()
	{
		currentState?.Update();
		CharacterUpdate();
	}

	private void FixedUpdate()
	{
		currentState?.FixedUpdate();
		CharacterFixedUpdate();
	}
	protected abstract void CharacterUpdate();
	protected abstract void CharacterFixedUpdate();

}
