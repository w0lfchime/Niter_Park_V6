using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    [Header("Meta")]
    public string name;

    [Header("Grounded Movement")]
    public float sneakSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float groundedControlForce;

    [Header("Airborne Movement")]
    public float airborneMaxSpeed;
    public float airborneTargetSpeedControlRate;
    public float airborneControlForce;

    [Header("Ground Checking")]
    public float groundCheckingDistance;
    public float isGroundedDistance;
    public float groundedSwitchCooldown;

    [Header("Rotation")]
    private float allowRotationPlayerSpeedFactor;
    private float allowRotationTimeAfterGrounding;
    private float rotationSpeed;

    [Header("Jump")]
    public float maxJumpHoldTime;
    public float jumpDirectionalInfluenceFactor;
    public int maxJumps;
    public float jumpForce;

    [Header("Custom Physics")]
    public float gravityTerminalVelocity; //Terminal Velocity 
    public float gravityFactor;
    public float gravityFactorWhileJumping; //How much effect gravity has while holding jump
    public float gravityFactorLerpRate; //How quickly the effect of gravity can change 
}
