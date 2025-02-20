
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
using System.Runtime.CompilerServices;

public enum CStateID //Standard state types
{
	//Debug
	PSNull,
	Suspended,
	Flight,
	//Gameplay
	OO_IdleGrounded,
	OO_IdleAirborne,
	OO_Walk,
	OO_Run,
	OO_Jump,
}

public abstract class Character : MonoBehaviour, IGameUpdate
{
	//======// /==/==/==/=||[FIELDS]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region fields
	//=//-----|General|-----------------------------------------------------------//=//
	#region general
	[Header("Meta")]
	public string characterInstanceName;
	public string characterStandardName;
	public bool nonPlayer = false;
	public Player player;

	[Header("Component Refs")]
	public Rigidbody rigidBody;
	public CapsuleCollider capsuleCollider;

	[Header("Debug")]
	public bool debug;
	public bool characterDebug = true;
	public Transform debugParentTransform;
	public TextMeshPro stateText;
	public VectorRenderManager vrm;
	public TextMeshPro debugTextPlus;

	[Header("Stats")]
	public CharacterStats ucs; //Universal
	public CharacterStats bcs; //Base Character Specific
	public CharacterStats acs; //Active Universal+Base 

	[Header("Time")]
	private int currentFrame = 0;
	#endregion general
	//=//-----|State|-------------------------------------------------------------//=//
	#region state
	[Header("State Machine")]
	public PerformanceCSM csm;
	public string currentStateName;
	public int stdMinStateDuration = 2;

	[Header("CSM debug")]
	public int requestQueueSize;
	public bool currStateExitAllowed;
	public bool currStatePriority;
	#endregion state
	//=//-----|Animation|---------------------------------------------------------//=//
	#region animation
	[Header("Animation Refs")]
	public Animator animator;
	public Transform rigAndMeshTransform;
	#endregion animation
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
	public bool isGroundedByState; //is the character grounded by state definition
	public bool onGrounding; //true a few frames after isgrounded is set to false to true
	public bool onUngrounding; //true a few frames after isgrounded is set from true to false
	public float distanceToGround;
	public float lastGroundedCheckTime = 0.0f;
	public float timeSinceLastGrounding = 0.0f;

	[Header("HandleNaturalRotation Variables")]
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
	//=//-----|Character Stats|---------------------------------------------------//=//
	#region character_stats
 

	#endregion character_stats
	//=//-------------------------------------------------------------------------//=//
	#endregion fields
	/////////////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[MONO]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region mono
	//=//-----|Event|------------------------------------------------------------//=//
	#region event
	private void Awake()
	{
		CharacterSetup();
		//...
	}
	private void Start()
	{
		CharacterStart();
		//...
	}
	private void OnEnable()
	{
		GameUpdateDriver.Register(this);
		//...
	}
	private void OnDisable()
	{
		//...
		GameUpdateDriver.Unregister(this);
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
		//...
		csm.PSMUpdate();
	}
	public void FixedFrameUpdate()
	{
		ProcessActionQueue();
		CharacterFixedFrameUpdate();
		//...
		csm.PSMFixedFrameUpdate();		
		CSMDebugUpdate(); //HACK:
	}
	private void FixedUpdate() // FixedPhysicsUpdate
	{
		//...
		HandleImpulseForce();
		HandleRegularForce();
		csm.PSMFixedPhysicsUpdate();
	}
	private void LateUpdate()
	{
		CharacterLateUpdate();
		//...
		csm.PSMLateUpdate();
	}
	#endregion updates
	//=//------------------------------------------------------------------------//=//
	#endregion mono
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//=//-----|Action Queue|-----------------------------------------------------//=//
	#region action_queue
	private void ProcessActionQueue()
	{
		currentFrame++;

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
	//=//-----|csm|--------------------------------------------------------------//=//
	#region csm
	/// <summary>
	/// For pushing states from states
	/// </summary>
	public void StatePushState(CStateID? stateID, int pushForce, int frameLifetime)
	{
		csm.PushState((CStateID)stateID, pushForce, frameLifetime);
	}
	private void CharacterPushState(CStateID? stateID, int pushForce, int frameLifetime)
	{
		csm.PushState((CStateID)stateID, pushForce, frameLifetime);
	}
	public void CSMDebugUpdate()
	{
		requestQueueSize = csm.requestQueue.Count;
		currStateExitAllowed = csm.currentState.exitAllowed == true;
	}
	#endregion csm
	//=//-----|Physics|----------------------------------------------------------//=//
	#region physics
	private void HandleRegularForce()
	{
		rigidBody.AddForce(appliedForce, ForceMode.Force);
		appliedForce = Vector3.zero;
	}
	private void HandleImpulseForce()
	{
		rigidBody.AddForce(appliedImpulseForce, ForceMode.Impulse);
		appliedImpulseForce = Vector3.zero;
	}
	#endregion physics
	//=//------------------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base 
	//Base methods and their helpers 
	//=//-----|Setup|------------------------------------------------------------//=//
	#region setup
	protected virtual void CharacterSetup()
	{
		//Local init functions
		SetMemberVariables();
		SetReferences();

		//The rest
		RegisterCommands();
		GlobalData.characterInitialized = true;	//HACK: do we need this?

		//magic numbers
		UpdateACD();

		//wiring data (runs in update)
		UpdateCharacterData();

		//state
		csm = new PerformanceCSM(this); //special init proc
		
		LogCore.Log("Character", $"Character initialized: {characterInstanceName}");

		if (csm.verified)
		{
			CharacterPushState(CStateID.Suspended, 9, 9); 
		}
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
		this.characterStandardName = GetType().Name;
		this.characterInstanceName = GetType().Name + "_1";

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
		//...


		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			SetDebug(!debug);
		}
		if (debug && Input.GetKeyDown(KeyCode.U))
		{
			UpdateACD();
		}
		//...
	}
	protected virtual void UpdateACD()
	{
		// ucd + bcd = acd

		if (ucs == null || bcs == null || acs == null)
		{
			Debug.LogError("Error: ucs, bcs, or acs null.");
			return;
		}

		// Get all fields of the ScriptableObject type
		FieldInfo[] fields = typeof(CharacterStats).GetFields(BindingFlags.Public | BindingFlags.Instance);

		foreach (var field in fields)
		{
			// Get the values from the first two objects
			object value1 = field.GetValue(ucs);
			object value2 = field.GetValue(bcs);

			// Handle addition for specific types
			if (value1 is int intValue1 && value2 is int intValue2)
			{
				field.SetValue(acs, intValue1 + intValue2);
			}
			else if (value1 is float floatValue1 && value2 is float floatValue2)
			{
				field.SetValue(acs, floatValue1 + floatValue2);
			}
			else if (value1 is Vector3 vectorValue1 && value2 is Vector3 vectorValue2)
			{
				field.SetValue(acs, vectorValue1 + vectorValue2);
			}
		}

		this.characterInstanceName = bcs.characterName;
	}
	protected virtual void UpdateCharacterData() //TODO: better name 
	{
		this.characterHeight = capsuleCollider.height;
	}
	#endregion data
	//=//-----|Mono|-------------------------------------------------------------//=//
	#region mono_abstracts
	protected abstract void CharacterAwake();
	protected abstract void CharacterStart();
	protected abstract void CharacterUpdate();
	protected abstract void CharacterFixedFrameUpdate();
	protected abstract void CharacterFixedPhysicsUpdate();
	protected abstract void CharacterLateUpdate();
	#endregion mono_abstracts
	//=//------------------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[DEBUG]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region debug
	//=//-----|General|----------------------------------------------------------//=//
	#region general
	public virtual void SetDebug(bool isEnabled)
	{
		//set debug
		debug = isEnabled;

		//set other debug components and what not
		debugParentTransform.gameObject.SetActive(enabled);
	}
	public virtual string CName(string message)
	{
		return $"{characterInstanceName}: {message}";
	}
	#endregion general
	//=//-----|State|------------------------------------------------------------//=//
	#region state
	public void OnStateSet()
	{
		currentStateName = csm.GetState().stateName;

		if (debug && stateText != null)
		{
			// Remove the character class name prefix if it exists
			if (currentStateName.StartsWith(characterStandardName))
			{
				currentStateName = currentStateName.Substring(characterStandardName.Length);
			}

			stateText.text = currentStateName;
		}
	}
	private void UpdateDebugText()
	{
		if (debugTextPlus != null)
		{
			//TODO: idk
		}
	}
	#endregion state
	//=//-----|Debug Vectors|----------------------------------------------------//=//
	#region debug_vectors
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
	#endregion debug_vectors
	//=//------------------------------------------------------------------------//=//
	#endregion debug 
	/////////////////////////////////////////////////////////////////////////////////////
}
