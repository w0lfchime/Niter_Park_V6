using UnityEngine;

public class GenericCharacter : Character
{
	//This is a character 


	protected override void CharacterStart()
	{
		//HACK
		//string playerName = "3 pushups, fucked by a black guy";

		//AppManager.Instance.AddPlayer(playerName);

		//this.player = AppManager.Instance.players[playerName];

		//player.character = this;
	}
	protected override void CharacterUpdate()
	{


	}

	protected override void CharacterFixedUpdate()
	{
		// Implement character-specific physics update logic here
	}

    protected override void CharacterLateUpdate()
    {
        
    }

    public override void CharacterInitialization()
	{
		base.CharacterInitialization();


		//Addtional code pretaining to character
	}

	public override void UpdateActiveCharacterData()
	{
		base.UpdateActiveCharacterData();

		this.name = "Generic";
	}
	protected override void RegisterCharacterStates()
	{
		base.RegisterCharacterStates();

		//common 
		stateDict.Add("IdleAirborne", new GenericIdleAirborne(this));
		stateDict.Add("IdleGrounded", new GenericIdleGrounded(this));
		stateDict.Add("Walk", new GenericWalk(this));
		stateDict.Add("Run", new GenericRun(this));
		stateDict.Add("Jump", new GenericJump(this));
		//(dev)
		stateDict.Add("Flight", new GenericFlight(this));
		

	}

}