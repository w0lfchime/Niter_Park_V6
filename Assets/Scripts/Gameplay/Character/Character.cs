using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;


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
	public VectorRenderManager vrm;

    [Header("Input")]
    public PlayerInputHandler inputHandler;
    private PlayerInput playerInput;

    [Header("Stats (Data)")]
	public CharacterData ucd; //Universal
	public CharacterData bcd; //Base Character Specific
	public CharacterData acd; //Active Universal+Base+Context 

	//Character 
	[Header("State")]
	public string currentState = "IdleAirborne";
	protected Dictionary<string, CharacterState> stateDict = new Dictionary<string, CharacterState>();
	protected PriorityQueue<string, int> stateQueue = new PriorityQueue<string, int>();

    [Header("Component Refs")]
	public Rigidbody rigidBody;
	public CapsuleCollider capsuleCollider;
	public Animator animator;

	[Header("Character Dimensions")]
	public float characterHeight;
	/// <summary>
	/// Physical State Wiring Data:
	/// </summary>

	[Header("Movement Variables")]
	public float currentMaxSpeed;
	public float currentControlForce;
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
	public Vector3 position;
	public Vector3 velocity;
	public float appliedGravityFactor; // ? what is this 
	public Vector3 appliedForce = Vector3.zero;
	public Vector3 appliedImpulseForce = Vector3.zero;

	[Header("Input Variables")]
	public Vector3 inputMoveDirection = Vector3.zero;
	public Vector3 inputLookDirection = Vector3.zero;

	public virtual void RegisterCommands()
	{
		if (GlobalData.characterInitialized)
		{
			//register commands here (?)

        }
	}

	public virtual void CharacterInitialization()
	{
		//meta
		this.name = bcd.name;

		//

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
		this.vrm = ServiceLocator.GetService<VectorRenderManager>();
		RegisterCommands();
        GlobalData.characterInitialized = true;

        //wiring data
        UpdateActiveCharacterData();

        //state
        stateDict = new Dictionary<string, CharacterState>();
        RegisterCharacterStates();

		//misc character data
		SetCharacterDimensions();

		LogCore.Log($"Character entered: {characterName}");
	}

	public virtual void SetCharacterDimensions()
	{
		this.characterHeight = capsuleCollider.height;
	}

    public virtual void DrawCharacterDebug()
	{


	}

	public virtual void ProcessInput()
	{
		//reset
		inputMoveDirection = Vector3.zero;
		inputLookDirection = Vector3.zero;


		//movement input
		if (inputHandler.GetButtonHold("MoveUp"))
		{
			inputMoveDirection += Vector3.up;
		}
        if (inputHandler.GetButtonHold("MoveRight"))
        {
			inputMoveDirection += Vector3.right;
        }
        if (inputHandler.GetButtonHold("MoveDown"))
        {
			inputMoveDirection += Vector3.down;
        }
        if (inputHandler.GetButtonHold("MoveLeft"))
        {
			inputMoveDirection += Vector3.left;
        }
		inputMoveDirection.Normalize();


		//"looking" input
        if (inputHandler.GetButtonHold("LookUp"))
        {
			inputLookDirection += Vector3.up;
        }
        if (inputHandler.GetButtonHold("LookRight"))
        {
			inputLookDirection += Vector3.right;
        }
        if (inputHandler.GetButtonHold("LookDown"))
        {
			inputLookDirection += Vector3.down;
        }
        if (inputHandler.GetButtonHold("LookLeft"))
        {
			inputLookDirection += Vector3.left;
        }
		inputLookDirection.Normalize();



		//developer input for flight mode  
		if (debug)
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				if (currentState == "Flight")
				{
					SetState("IdleAirborne");
				} else
				{
					SetState("Flight");
				}
			}
			if (Input.GetKeyDown(KeyCode.U))
			{
				UpdateActiveCharacterData();
			}
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			debug = !debug;
		}

	}

	//Physics






	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -



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

		LogCore.Log($"Current state: {currentState}");
    }


    /// <summary>
    /// MonoBehavior 
    /// </summary>
	/// 
	private void Awake()
    {
		CharacterInitialization();
		CharacterStart();

	}
    private void Start()
	{


    }
	private void Update()
	{
        inputHandler.UpdateInputs();
		ProcessInput();
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
		CharacterFixedUpdate();
        stateDict[currentState]?.FixedUpdate();

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
