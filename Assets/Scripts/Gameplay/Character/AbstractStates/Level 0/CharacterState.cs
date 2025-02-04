using UnityEngine;

public abstract class CharacterState
{
    [Header("Parent")]
    protected Character ch;

    [Header("Component Refs")]
    protected Animator anim;
    protected Rigidbody rb;
    protected CapsuleCollider cc;
    protected PlayerInputHandler pinput;

    public CharacterState(Character character)
    {
        this.ch = character;
        this.anim = character.animator;
        this.rb = character.rigidBody;
        this.cc = character.capsuleCollider;
        this.pinput = character.inputHandler; 
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    //data copy method ?

}

