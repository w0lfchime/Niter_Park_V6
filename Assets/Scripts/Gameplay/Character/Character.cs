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
	public Transform debugParentTransform;
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
	[Header("State Variable")]
	public string currentState = null;
	protected Dictionary<string, CharacterState> stateDict = new Dictionary<string, CharacterState>();
	protected Queue<KeyValuePair<string, int>> stateQueue = new Queue<KeyValuePair<string, int>>();
	public bool stateSwitchLastFrame = false;

	[Header("State Parameter")]
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
	
	public virtual void SetMemberVariables()
	{
		//meta
		this.characterName = bcd.characterName;

        //debug 
        this.debug = GlobalData.debug;
    }

	public virtual void SetReferences()
	{
		//input
		playerInput = GetComponent<PlayerInput>();
		inputHandler = new PlayerInputHandler(playerInput);

        //physics
        rigidBody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();

        //animation
        animator = GetComponent<Animator>();

        //debug
        debugParentTransform = transform.Find("Debug");
        stateText = debugParentTransform.Find("CharacterStateText")?.GetComponent<TextMeshPro>();

        this.vrm = ServiceLocator.GetService<VectorRenderManager>();
    }
	public virtual void CharacterInitialization()
	{
		//Local init functions
		SetMemberVariables();
		SetReferences();

		//The rest
		RegisterCommands();
        GlobalData.characterInitialized = true;	//HACK: do we need this?

        //wiring data
        UpdateActiveCharacterData();

        //state
        RegisterCharacterStates();
		LogCore.Log("InitHighDetail", $"Regsitered character states. Number of states: {stateDict.Count}");

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

		//TODO: register flight state 

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

		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			SetDebug(!debug);
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

	/// <summary>
	/// Checks passed state for null or not registered. Does the same thing as check current state.
	/// </summary>
	/// <param name="state"></param>
	/// <returns></returns>
    public bool ApproveState(string state)
    {

        if (state == null)
        {
            CLog("StateError", "Attempted approval a null state. Rejected.");
            return false;
        }
		if (state.Length < 4)
		{
			CLog("StateError", $"Attempted approval of an empty or overly short state: ->{state}<- Rejected.");
			return false;
		}
        if (!stateDict.ContainsKey(state))
        {
            CLog("StateError", $"Attempted approval of state: ->{state}<- which does not exist in stateDict. Rejected.");
            return false;
        }
		CLog("StateHighDetail", $"Approved state: ->{state}<-");


        return true;
    }
    public void CheckCurrentState()
    {

        if (!ApproveState(currentState))
        {
            CLog("StateError", "Current state is invalid. Setting state to Suspended.");
            TrySetState("Suspended", 0);
        }
    }

    public void ProcessStateQueue()
	{
		if (stateQueue.Count > 0)
		{
			string newState = null;

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
					newState = state.Key;
					CLog("StateHighDetail", $"Top priority: {currentState}");
				}
				//dec limit
				limit--;
			}

			//finally, after being approved, then being dequeued, set the state.
			SetState(newState);
		} else
		{
			//Set stateSwitchLastFrame to false, since state queue was empty. it is enabled on SetState.
			stateSwitchLastFrame = false;
		}

	}

	private void SetState(string newState)
	{
		//set state will typically be called in LateUpdate, so enable stateSwitchLastFrame for the next frame.
		stateSwitchLastFrame = true;

        stateDict[currentState].Exit();
		string oldState = currentState;
        currentState = newState;
        stateDict[currentState].Enter();

		CLog("StateHighDetail", $"Switched to from {oldState} to {currentState}");
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





	// Debug - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
	public virtual void SetDebug(bool enabled)
	{
		//set debug
		debug = enabled;

		//set other debug components and what not
		debugParentTransform.gameObject.SetActive(enabled);
	}

	//create override with position offset?
	public void UpdateDebugVector(string name, Vector3 vector, Color color)
	{
		if (debug)
		{
			vrm.UpdateVector(name, debugParentTransform, Vector3.zero, vector, color);
		}
	
	}
    public void StampDebugVector(string name, Vector3 vector, Color color)
	{
		if (debug)
		{
			vrm.StampVector(name, transform.position, vector, color, 1.0f);
		}
	}

    public virtual void DrawCharacterDebug()
    {
		if (debug)
		{

		}
		if (stateSwitchLastFrame && stateText != null)
		{
			//source directly for debugging certainty
            string currentStateName = stateDict[currentState].GetType().Name;

            if (currentStateName.Length > characterName.Length)
            {
                if (currentStateName.Substring(0, characterName.Length) == characterName)
                {
                    currentStateName = currentStateName.Substring(characterName.Length);
                }
            }

            stateText.text = currentStateName;
            
        }

    }

	public virtual void CLog(string category, string message)
	{
		string messageWithName = $"{characterName}: {message}";
		LogCore.Log(category, message);
	}


    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

}
