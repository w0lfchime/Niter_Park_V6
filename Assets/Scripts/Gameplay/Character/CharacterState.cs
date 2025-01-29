using UnityEngine;

public abstract class CharacterState
{
    protected Character character;

    public CharacterState(Character character)
    {
        this.character = character;
        this.rb = character.rigidBody;
    }

    protected Rigidbody rb;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    //data copy method ?


    public virtual void ProcessInput() { }
    public virtual void DataUpdates() { }
    public virtual void Animate() { }
}

