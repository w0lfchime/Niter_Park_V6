using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Game/CharacterStats", order = 1)]
public class CharacterStats : ScriptableObject
{
    [Header("Meta")]
    public string characterName;

	[Header("Movement")]
	public float gSneakSpeed = 10;
	public float gWalkSpeed = 20;
	public float gRunSpeed = 30;
	public float gAccFactor = 0.5f;
	public float aDriftSpeed = 10;
	public float aAccFactor = 0.5f;

	[Header("Ground Checking")]
	public float groundCheckingDistance = 5;
	public float isGroundedDistance = 0.2f;
	public float groundedSwitchCooldown = 0.2f;

	[Header("HandleNaturalRotation")]
	public float allowRotationPlayerSpeedFactor = 0;
	public float allowRotationTimeAfterGrounding = 0;
	public float rotationSpeed = 0;

	[Header("Jump")]
	public float maxJumpHoldTime = 0.5f;
	public float jumpDirectionalInfluenceFactor = 0.5f;
	public int maxJumps = 2;
	public float jumpForce = 25.0f;

	[Header("Physics")]
	public float gravityTerminalVelocity = -25.0f; //Terminal Velocity 
	public float gravityForceFactor = 0;
	public float gravityFactorWhileJumping = 0.004f; //How much effect gravity has while holding jump
	public float gravityFactorLerpRate = 0.1f; //How quickly the effect of gravity can change

	[Header("Flight")] //debug mode of character 
    public float meowidk;

}
