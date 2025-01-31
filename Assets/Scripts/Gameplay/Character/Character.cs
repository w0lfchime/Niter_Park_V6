using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting.FullSerializer;
using System;


public abstract class Character : MonoBehaviour
{
	[Header("Meta")]
	public string characterName;
	public bool nonPlayer = false;
	public Player player;

	[Header("Debug")]
	public bool debug;
	public bool characterDebug = true;
	public TextMeshPro stateText;

    [Header("Input")]
    public PlayerInputHandler inputHandler;
    private PlayerInput playerInput;

    [Header("Stats (Data)")]
	public CharacterData ucd; //Universal
	public CharacterData bcd; //Base Character Specific
	public CharacterData acd; //Active Universal+Base+Context 

	//Character 
	[Header("States")]
	protected string currentState;
	protected Dictionary<string, CharacterState> stateDict = new Dictionary<string, CharacterState>();

	[Header("Component Refs")]
	public Rigidbody rigidBody;
	public CapsuleCollider capsuleCollider;
	public Animator animator;

	/// <summary>
	/// Physical State Wiring Data:
	/// </summary>

	[Header("Movement Variables")]
	public float currentMaxSpeed;
	public float currentControlForce;
	public Vector3 movementInput;
	public Vector3 playerMovementDirection;
	public float playerSpeed;
	public float velocityX;
	public float velocityY;

	[Header("Ground Checking Variables")]
	public LayerMask groundLayer;
	public bool isGrounded;
	public bool onGrounding; //on frame isgrounded is set to false to true
	public bool onUngrounding; //on frame isgrounded is set from true to false
	public float distanceToGround;
	public float lastGroundedCheckTime = 0.0f;
	public float timeSinceLastGrounding = 0.0f;

	[Header("Rotation Variables")]
	public bool clockwiseRotation = true;
	public bool facingRight;

	[Header("Jump Variables")]
	public float lastJumpTime;
	public int jumpCount;
	public float jumpForceLerp;
	public bool jumpAllowedByContext = true;

	[Header("Physics Variables")]
	public float appliedGravityFactor;
	public Vector3 targetVelocity = Vector3.zero;
	public Vector3 appliedForce = Vector3.zero;
	public Vector3 appliedImpulseForce = Vector3.zero;


	public virtual void RegisterCommands()
	{
		if (GlobalData.characterInitialized)
		{



		}
	}

	public virtual void CharacterInitialization()
	{
		//meta
		this.name = bcd.name;

		//input
		playerInput = GetComponent<PlayerInput>();
		inputHandler = new PlayerInputHandler(playerInput);

        //physics
        rigidBody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();

		//animation
		animator = GetComponent<Animator>();

		//debug
		stateText = transform.Find("CharacterStateText")?.GetComponent<TextMeshPro>();
		this.debug = GlobalData.debug;
		RegisterCommands();
        GlobalData.characterInitialized = true;

        //character function
        UpdateActiveCharacterData();
		RegisterCharacterStates();



		LogCore.Log($"Character entered: {characterName}");
	}

    public virtual void DrawCharacterDebug()
	{

		//more gizmo draw data
	}

	public virtual void UpdateActiveCharacterData()
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

		this.name = bcd.name;
	}

	public void SetState(string newState)
	{
        if (currentState != null)
        {
			stateDict[currentState]?.Exit();            
        }
		currentState = newState;
		stateDict[currentState]?.Enter();


		//debug
		if (debug && stateText != null)
		{

			string currentStateName = stateDict[currentState].GetType().Name;
			stateText.text = currentStateName.Substring(name.Length);
		}
    }


    /// <summary>
    /// MonoBehavior 
    /// </summary>
    private void Start()
	{
		//HACK: ? may be sub optimal function placement 
		CharacterInitialization();
		CharacterStart();
	}
	private void Update()
	{
        inputHandler.UpdateInputs();
		if (currentState == null || !stateDict.ContainsKey(currentState))
		{
			LogCore.Log("Null or undefined state error. Setting state to IdleAirborne.");
			SetState("IdleAirborne");
		}



		CharacterUpdate();
		stateDict[currentState]?.Update();
	}
	private void FixedUpdate()
	{
        stateDict[currentState]?.FixedUpdate();
		CharacterFixedUpdate();
	}
	private void OnDisable() { }
	private void OnEnable() { }

	/// <summary>
	/// Monobehavior Abstracts 
	/// </summary>
	protected abstract void CharacterStart();
	protected abstract void CharacterUpdate();
	protected abstract void CharacterFixedUpdate();

	/// <summary>
	/// Other abstracts
	/// </summary>
	protected abstract void RegisterCharacterStates();

}
