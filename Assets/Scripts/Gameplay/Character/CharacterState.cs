using UnityEngine;

public abstract class CharacterState
{
    [Header("Parent")]
    protected Character ch;

    [Header("Component Refs")]
    protected Animator animator;
    protected Rigidbody rb;
    protected CapsuleCollider cc;

    public CharacterState(Character character)
    {
        this.ch = character;
        this.animator = character.animator;
        this.rb = character.rigidBody;
        this.cc = character.capsuleCollider;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    //data copy method ?


    public virtual void ProcessInput() { }
    public virtual void DataUpdates() { }
    public virtual void Animate() { }
}

