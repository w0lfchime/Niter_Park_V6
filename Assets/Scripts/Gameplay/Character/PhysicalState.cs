using UnityEngine;

public class PhysicalState : CharacterState
{
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
    public bool onUngrounding; //on frame isgrounded isss set from true to false
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
    public float appliedGravityFactor; //gravity factor thats actually used. Post lerp. 
    public Vector3 appliedForce;
    public Vector3 appliedImpulseForce;

    public PhysicalState(Character character) : base(character)
    {

    }



    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        //base.Update();

        DataUpdates();
        ProcessInput();
        Animate();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    public override void ProcessInput()
    {
        base.ProcessInput();
        //access input manager, request input 
    }

    public override void DataUpdates()
    {
        base.DataUpdates();

    }

    public override void Animate()
    {
        base.Animate();

    }

    // - - - - - - - - 



    void ApplyMotion()
    {
        rb.AddForce(appliedForce, ForceMode.Force);
        rb.AddForce(appliedImpulseForce, ForceMode.Impulse);
    }
}
