using UnityEngine;

public class GenericCharacter : Character
{
    //This is a character 


    protected override void CharacterStart()
    {
        
    }
    protected override void CharacterUpdate()
	{
		
	}

	protected override void CharacterFixedUpdate()
	{
		// Implement character-specific physics update logic here
	}

	public override void CharacterInitialization()
	{
		base.CharacterInitialization();

		//Addtional code pretaining to character
	}
	protected override void RegisterCharacterStates()
	{

	}

}