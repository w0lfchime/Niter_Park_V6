
//----------------------------------------------------------------------------------
// Copyright (2025) NITER
//
// This code is part of the PARK-v6 Unity Framework. It is proprietary software.  
// Unauthorized use, modification, or distribution is not permitted without  
// explicit permission from the owner.  
//----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine.InputSystem;
using System;

public enum CState //Standard state types
{
	IdleGrounded,
	IdleAirborne,
	Walk,
	Run,
	Jump,
	COUNT
}

public abstract class Character : MonoBehaviour
{
	//======// /==/==/==/=||[FIELDS]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region fields
	//=//-----|General|-----------------------------------------------------------//=//
	#region general
	[Header("Meta")]
	public string characterName;
	public string characterClassName;
	public bool nonPlayer = false;
	public Player player;

	[Header("Component Refs")]
	public Rigidbody rigidBody;
	public CapsuleCollider capsuleCollider;
	public Animator animator;

	[Header("Debug")]
	public bool debug;
	public bool characterDebug = true;
	public Transform debugParentTransform;
	public TextMeshPro stateText;
	public VectorRenderManager vrm;

	[Header("Stats (Data)")]
	public CharacterData ucd; //Universal
	public CharacterData bcd; //Base Character Specific
	public CharacterData acd; //Active Universal+Base 

	[Header("Time")]
	private int currentFrame = 0;
	#endregion general
	//=//-----|Input|-------------------------------------------------------------//=//
	#region input
	[Header("Input")]
	public PlayerInputHandler inputHandler;
	private PlayerInput playerInput;

	[Header("Input Variables")]
	public Vector3 inputMoveDirection = Vector3.zero;
	public Vector3 inputLookDirection = Vector3.zero;
	#endregion input
	//=//-----|Action Queue|------------------------------------------------------//=//
	#region action_queue


	[Header("Action Queue")]
	private readonly Queue<(int frame, Action action)> actionQueue = new();
	private readonly Queue<(int frame, Action<object> action, object param)> paramActionQueue = new();
	#endregion action_queue
	//=//-----|Gameplay Data|-----------------------------------------------------//=//
	#region gameplay_data
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
	public bool isGrounded; //is the capsule physically 'grounded'
	public bool isGroundedBystate; //is the character grounded by state definition
	public bool onGrounding; //true a few frames after isgrounded is set to false to true
	public bool onUngrounding; //true a few frames after isgrounded is set from true to false
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

	[Header("Physics Variables")]
	public Vector3 position;
	public Vector3 velocity;
	public float appliedGravityFactor; // ? what is this 
	public Vector3 appliedForce = Vector3.zero;
	public Vector3 appliedImpulseForce = Vector3.zero;
	#endregion gameplay_data
	//=//-----|State|-------------------------------------------------------------//=//
	#region state
	[Header("State")]
	public string currentState = null;
	protected CharacterState[] stateArray = new CharacterState[(int)CState.COUNT];
	protected StateSetRequestHeap<StateSetRequest> stateHeap = new();

	protected struct StateSetRequest
	{
		public string StateName;
		public int PushForce;
		public int ExpirationFrame;
		public bool ClearOnStateSwitch;
		public int PushFrame; // Used for LIFO sorting

		public StateSetRequest(string stateName, int pushForce, int expirationFrame, bool clearOnStateSwitch, int pushFrame)
		{
			StateName = stateName;
			PushForce = pushForce;
			ExpirationFrame = expirationFrame;
			ClearOnStateSwitch = clearOnStateSwitch;
			PushFrame = pushFrame;
		}
	}

	[Header("State Parameter")]
	public int perFrameStateQueueLimit = 10;
	#endregion state
	//=//-------------------------------------------------------------------------//=//
	#endregion fields
	/////////////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[MONO]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region mono
	//=//-----|Event|------------------------------------------------------------//=//
	#region event
	private void Awake()
	{
		CharacterInitialization();
		CharacterStart();

	}
	private void Start()
	{
		CheckCurrentState();

	}
	private void OnDisable()
	{

	}
	private void OnEnable()
	{

	}
	#endregion event
	//=//-----|Updates|----------------------------------------------------------//=//
	#region updates
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
		currentFrame++; //used by action queue

		ProcessActionQueue();

		CharacterFixedUpdate();
		stateDict[currentState]?.FixedUpdate();

		ProcessStateHeap();
	}
	private void LateUpdate()
	{
		CharacterLateUpdate();

		stateDict[currentState]?.LateUpdate();
	}
	#endregion updates
	//=//-----|Abstracts|--------------------------------------------------------//=//
	#region abstracts
	protected abstract void CharacterAwake();
	protected abstract void CharacterStart();
	protected abstract void CharacterUpdate();
	protected abstract void CharacterFixedUpdate();
	protected abstract void CharacterLateUpdate();
	#endregion abstracts
	//=//------------------------------------------------------------------------//=//
	#endregion mono
	/////////////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[LOCAL]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//=//-----|Action Queue|-----------------------------------------------------//=//
	#region action_queue
	private void ProcessActionQueue()
	{
		// Execute non-param actions
		while (actionQueue.Count > 0 && actionQueue.Peek().frame <= currentFrame)
		{
			var (_, action) = actionQueue.Dequeue();
			action?.Invoke();
		}
		// Execute param actions
		while (paramActionQueue.Count > 0 && paramActionQueue.Peek().frame <= currentFrame)
		{
			var (_, action, param) = paramActionQueue.Dequeue();
			action?.Invoke(param);
		}
	}
	public void ScheduleAction(int framesFromNow, Action action)
	{
		if (framesFromNow <= 0)
		{
			action?.Invoke();
			return;
		}

		actionQueue.Enqueue((currentFrame + framesFromNow, action));
	}
	public void ScheduleAction<T>(int framesFromNow, Action<T> action, T param)
	{
		if (framesFromNow <= 0)
		{
			action?.Invoke(param);
			return;
		}

		paramActionQueue.Enqueue((currentFrame + framesFromNow, (p) => action((T)p), param));
	}
	#endregion action_queue
	//=//-----|Debug|------------------------------------------------------------//=//	
	#region debug
	public virtual void SetDebug(bool isEnabled)
	{
		//set debug
		debug = isEnabled;

		//set other debug components and what not
		debugParentTransform.gameObject.SetActive(enabled);
	}
	public virtual void CLog(string category, string message)
	{
		string messageWithName = $"{characterName}: {message}";
		LogCore.Log(category, message);
	}
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
	#endregion debug 
	//=//------------------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[STATE MACHINE]||==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region state_machine
	//=//-----|Debug & Safety|---------------------------------------------------//=//
	#region debug_and_safety
	public virtual void VerifyCharacterStates()
	{
		bool passed = true;
		foreach (string key in stateDict.Keys)
		{
			if (!stateDict[key].VerifyState())
			{
				CLog("Critical", $"State {key} is invlaid.");
				passed = false;
			}
		}
		if (passed)
		{
			CLog("CharacterSetupHighDetail", "Successfully verified all registered character states.");
		}
		else
		{
			CLog("Critical", "Failed to verify all registered character states.");
		}
	}
	private bool ApproveState(string state)
	{
		if (state == null)
		{
			CLog("CharacterStateError", "Attempted approval a null state. Rejected.");
			return false;
		}
		if (state.Length < 4)
		{
			CLog("CharacterStateError", $"Attempted approval of an empty or oddly named state: ->{state}<- Rejected.");
			return false;
		}
		if (!stateDict.ContainsKey(state))
		{
			CLog("CharacterStateError", $"Attempted approval of state: ->{state}<- which does not exist in stateDict. Rejected.");
			return false;
		}
		CLog("CharacterStateHighDetail", $"Approved state: ->{state}<-");

		return true;
	}
	private void CheckCurrentState()
	{
		if (!ApproveState(currentState))
		{
			CLog("CharacterStateError", "Current state is invalid. Setting state to Suspended.");
			PushState("Suspended", 0);
		}
	}
	#endregion debug_and_safety
	//=//-----|Public|-----------------------------------------------------------//=//
	#region public
	public CharacterState getState(string stateName)
	{
		return stateDict[stateName];
	}
	public void PushState(string stateName, int pushForce, int lifetime, bool clearOnStateSwitch)
	{
		int expirationFrame = currentFrame + lifetime;
		var newRequest = new StateSetRequest(stateName, pushForce, expirationFrame, clearOnStateSwitch, currentFrame);

		// Add new state push to the heap (sorted by push force, then LIFO)
		stateHeap.Enqueue(newRequest, pushForce, currentFrame);

		// Immediate override if the new push force is greater than the current state's priority
		if (currentState == null || pushForce > stateDict[currentState].GetPriority())
		{
			SwitchToState(newRequest);
		}
	}
	public void ForceState(string stateName)
	{
		if (stateDictionary.TryGetValue(stateName, out var newState))
		{
			currentState = newState;
			currentState.OnEnter();
			stateHeap.Clear();
		}
	}
	#endregion public 
	//=//-----|Processing|-------------------------------------------------------//=//
	private void ProcessStateHeap()
	{
		// Remove expired state pushes
		while (stateHeap.Count > 0 && stateHeap.Peek().ExpirationFrame <= currentFixedFrame)
		{
			stateHeap.Dequeue();
		}

		if (stateHeap.Count == 0) return;

		// Get the highest-priority state (max push force, LIFO tie-breaker)
		StateRequest bestRequest = stateHeap.Peek();

		if (currentState == null || bestRequest.PushForce > currentState.GetPriority())
		{
			SwitchToState(bestRequest);
		}
	}
	private void SwitchToState(StateSetRequest newRequest)
	{
		if (stateDictionary.TryGetValue(newRequest.StateName, out var newState))
		{
			currentState = newState;
			currentState.OnEnter();

			// If the new state forces a clear, wipe the heap
			if (currentState.ForceClearStateHeapOnEntry)
			{
				stateHeap.Clear();
			}
			else
			{
				// Remove states where ClearOnStateSwitch = false
				var tempHeap = new MaxHeap<StateRequest>();
				while (stateHeap.Count > 0)
				{
					var request = stateHeap.Dequeue();
					if (request.ClearOnStateSwitch) tempHeap.Enqueue(request, request.PushForce, request.PushFrame);
				}
				stateHeap = tempHeap;
			}
		}
	}
	public void ForceState(string stateName)
	{
		if (stateDictionary.TryGetValue(stateName, out var newState))
		{
			currentState = newState;
			currentState.OnEnter();
			stateHeap.Clear();
		}
	}
	//Finally,
	private void SetState(string newState)
	{
		string oldState = currentState;
		stateDict[currentState].Exit();
		currentState = newState;
		stateDict[currentState].Enter();

		CLog("CharacterStateHighDetail", $"Switched to from ->{oldState}<- to ->{currentState}<-");

		if (debug && stateText != null)
		{
			// Get the current state name from the dictionary
			string currentStateName = stateDict[currentState].GetType().Name;

			// Remove the character class name prefix if it exists
			if (currentStateName.StartsWith(characterClassName))
			{
				currentStateName = currentStateName.Substring(characterClassName.Length);
			}

			// Update the UI text
			stateText.text = currentStateName;
		}
	}

	//=//------------------------------------------------------------------------//=//
	#endregion state_machine
	/////////////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[BASE]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base 
	//Base methods and their helpers 

	//=//-----|Setup|------------------------------------------------------------//=//
	#region setup
	protected virtual void CharacterInitialization()
	{
		//Local init functions
		SetMemberVariables();
		SetReferences();

		//The rest
		RegisterCommands();
		GlobalData.characterInitialized = true;	//HACK: do we need this?

		//wiring data
		UpdateActiveCharacterData();

		//misc character data
		SetCharacterDimensions();

		//state
		RegisterCharacterStates();
		LogCore.Log("CharacterSetupHighDetail", $"Regsitered character states. Number of states: {stateDict.Count}");

		//Testing and Verification 
		VerifyCharacterStates();
		
		LogCore.Log("Character", $"Character initialized: {characterName}");
	}
	protected virtual void RegisterCommands()
	{
		if (GlobalData.characterInitialized)
		{
			//register commands here (?)

		}
	}
	protected virtual void SetMemberVariables()
	{
		//meta
		this.characterName = bcd.characterName;
		this.characterClassName = GetType().Name;

		//state
		currentState = null;
		//HACK: start as null to trigger check state to set as suspended state


		//debug 
		this.debug = GlobalData.debug;
	}
	protected virtual void SetReferences()
	{
		//input
		playerInput = GetComponent<PlayerInput>();
		inputHandler = new PlayerInputHandler(playerInput);

		//physics
		rigidBody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		groundLayer = LayerMask.GetMask("Ground");

		//animation
		animator = GetComponent<Animator>();

		//debug
		debugParentTransform = transform.Find("Debug");
		stateText = debugParentTransform.Find("CharacterStateText")?.GetComponent<TextMeshPro>();

		this.vrm = ServiceLocator.GetService<VectorRenderManager>();
	}
	private void ResgisterState()
	{

	}
	protected virtual void RegisterCharacterStates()
	{
		foreach (CState stateEnum in Enum.GetValues(typeof(CState)))
		{
			string className = $"{this.characterName}{stateEnum}";  // "GenericIdleAirborne", etc.
			Type classType = Type.GetType(className);

			if (classType != null)
			{
				CharacterState stateInstance = (CharacterState)Activator.CreateInstance(classType, this);
				RegisterState(stateEnum, stateInstance);
			}
			else
			{
				LogCore.Log("CharacterSetup", $"State class not found: {className}");
			}
		}
	}
	#endregion setup
	//=//-----|Data|-------------------------------------------------------------//=//
	#region data
	protected virtual void ProcessInput()
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
					PushState("IdleAirborne", 5);
				} else
				{
					PushState("Flight", 5);
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
	protected virtual void SetCharacterDimensions()
	{
		this.characterHeight = capsuleCollider.height;
	}
	protected virtual void UpdateActiveCharacterData()
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
	protected virtual void UpdateCharacterData() //TODO: better name 
	{

	}
	#endregion data


	#endregion base





}
