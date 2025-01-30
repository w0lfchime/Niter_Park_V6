using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine.TextCore.Text;


public abstract class Character : MonoBehaviour
{
	[Header("Meta")]
	public string characterName;
	public Player player;

	[Header("Debug")]
	public bool characterDebug = true;
	public TextMeshPro stateText;
	//giving character class service access?

	[Header("Stats (Data)")]
	public CharacterData ucd; //Universal
    public CharacterData bcd; //Base Character Specific
    public CharacterData acd; //Active Universal+Base+Context 

	//Character 
	[Header("States")]
	private CharacterState currentState;
	private Dictionary<string, CharacterState> stateDict = new Dictionary<string, CharacterState>();

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
    public float targetVelocityX;
    public float targetVelocityY;

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

    [Header("Physics Variables")]
    public float appliedGravityFactor; 
    public Vector3 appliedForce;
    public Vector3 appliedImpulseForce;

    public virtual void DrawCharacterDebug()
    {
        if (stateText != null)
        {
            stateText.text = $"State: {currentState}"; 
        }

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
	}

	public virtual void CharacterInitialization()
	{
		this.name = bcd.name;

		LogCore.Log($"Character entered: {characterName}");

		//physics
		rigidBody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();

		//animation
		animator = GetComponent<Animator>();

		//debug
		stateText = transform.Find("CharacterStateText")?.GetComponent<TextMeshPro>();



        UpdateActiveCharacterData();

		RegisterCharacterStates();
	}



	public void SetState(string newState)
	{

	}


	/// <summary>
	/// MonoBehavior 
	/// </summary>
    private void Start()
    {

		CharacterStart();
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
