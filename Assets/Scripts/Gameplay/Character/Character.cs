using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;


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
	protected Queue<KeyValuePair<string, int>> stateQueue = new Queue<KeyValuePair<string, int>>();
	public int queueSize = 0;
	public int perFrameStateQueueLimit = 10;

    [Header("Component Refs")]
	public Rigidbody rigidBody;
	public CapsuleCollider capsuleCollider;
	public Animator animator;

	[Header("Character Dimensions")]
	public float characterHeight;




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







    // Character Setup - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
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
		this.characterName = bcd.characterName;

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
        RegisterCharacterStates();
		LogCore.Log("C-InitHighDetail", $"Regsitered character states.");


		//misc character data
		SetCharacterDimensions();
		
		LogCore.Log("Match", $"Character entered: {characterName}");
    }

    /// <summary>
	/// Some states have truly universal implementation and can be registered in base character class
	/// </summary>
	protected virtual void RegisterCharacterStates()
	{
		stateDict.Add("Suspended", new SuspendedState(this));

		//other...
	}


    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 




    // Character Data - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
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

		// DEBUG: 
		//developer input for flight mode  
		if (debug)
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				if (currentState == "Flight")
				{
					TrySetState("IdleAirborne", 5);
				} else
				{
					TrySetState("Flight", 5);
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
	public virtual void SetCharacterDimensions()
	{
		this.characterHeight = capsuleCollider.height;
	}
	public virtual void UpdateActiveCharacterData()
	{
		// ucd + bcd = acd

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

		this.characterName = bcd.characterName;


		// Other data
	}
	public virtual void UpdateCharacterData()
	{
        this.queueSize = stateQueue.Count;
    }
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 






    // State Control - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


	/// <summary>
	/// Attempts to set the state of the character. The state must first be approved, then 
	/// have the highest priority in the queue at the end of the frame. 
	/// Priorities:
	/// 0, 1, 2 - lowest importance. basic defaulting and error prevention, etc..
	/// 3, 4 - general states.
	/// 5, 6, 7 - important states. Responsive controls, grounding, etc..
	/// 8, 9, 10 - critical states. Damage, flinching, getting grabbed, etc..
	/// 10+ - complete overuling states. cutscenes, game ending, dev tools, etc..
	/// </summary>
	/// <param name="newState"></param>
	/// <param name="priority"></param>
    public void TrySetState(string newState, int priority)
	{
		if (!ApproveState(newState))
		{
			return;
		}
	
		stateQueue.Enqueue(new KeyValuePair<string, int>(newState, priority));
		CLog("StateHighDetail", $"Enqueued state: {newState} with priority {priority}");
    }
    public bool ApproveState(string state)
    {

        if (state == null)
        {
            CLog("StateError", "Attempted to check a null state. Ignoring.");
            return false;
        }
        else if (!stateDict.ContainsKey(state))
        {
            CLog("StateError", $"Attempted use of state: {state} which does not exist in stateDict.");
            return false;
        }
		CLog("StateHighDetail", $"Approved state: {state}");


        return true;
    }

    public void ProcessStateQueue()
	{
		if (stateQueue.Count > 0)
		{
			int limit = perFrameStateQueueLimit;
			int topPriority = 0;

			CLog("StateHighDetail", "Processing stateQueue ->");

			while (stateQueue.Count > 0 && limit > 0)
			{
				//dequeue state
				KeyValuePair<string, int> state = stateQueue.Dequeue();

				CLog("StateHighDetail", $"Dequeued state {state.Key}");
				//check if highest yet prio
				if (state.Value >= topPriority)
				{
					topPriority = state.Value;
					currentState = state.Key;
					CLog("StateHighDetail", $"Current state set to {currentState}");
				}
				//dec limit
				limit--;
			}

			CheckCurrentState();
		}

	}

	public void CheckCurrentState()
	{
		bool invalidState = false;
		if (currentState == null)
		{
			invalidState = true;

			CLog("StateError", "Current state is null.");
		} 
		else if (!stateDict.ContainsKey(currentState))
		{
			invalidState = true;

			CLog("StateError", $"Current state: {currentState} does not exist in stateDict.");
		}

		//finally
		if (invalidState)
		{
			CLog("StateError", "Current state is invalid. Setting state to Suspended.");
			TrySetState("Suspended", 0);
		}
	}
	private void SetState(string newState)
	{

		CheckCurrentState();
        stateDict[currentState].Exit();
		string oldState = currentState;
        currentState = newState;
        stateDict[currentState].Enter();

		CLog("StateSwitch", $"Switched to from {oldState} to {currentState}");

        //debug
        if (debug && stateText != null)
        {
            string currentStateName = stateDict[currentState].GetType().Name;
			if (currentStateName.Substring(0, characterName.Length) == characterName) 
			{
				currentStateName = currentStateName.Substring(characterName.Length);
			}
			stateText.text = currentStateName;
        } 
		else
		{
			print("HERE!!! ITS HERE!!!"); //lmfaoooo
		}

    }
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 



    // Monobehaviour - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    private void Awake()
    {
		CharacterInitialization();
		CharacterStart();

	}
    private void Start()
	{
		CheckCurrentState();

    }
	private void Update()
	{
        inputHandler.UpdateInputs();
		ProcessInput();
		UpdateCharacterData();
		CharacterUpdate();
		stateDict[currentState]?.Update();
	}
	private void FixedUpdate()
	{
		CharacterFixedUpdate();
        stateDict[currentState]?.FixedUpdate();

	}
    private void LateUpdate()
    {
		CharacterLateUpdate();

        ProcessStateQueue();
    }
    private void OnDisable() 
	{
	
	}
	private void OnEnable()
	{
	
	}
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 


    // Monobehavior Abstracts - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  
    protected abstract void CharacterStart();
	protected abstract void CharacterUpdate();
	protected abstract void CharacterFixedUpdate();
	protected abstract void CharacterLateUpdate();
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 





    // Misc Debug - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
    public virtual void DrawCharacterDebug()
    {


    }
	public virtual void CLog(string category, string message)
	{
		string messageWithName = $"{characterName}: {message}";
		LogCore.Log(category, message);
	}

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

}
