
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

public class ExampleCharacter : Character
{
	//======// /==/==/==/=||[FIELDS]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region fields
	//=//-------------------------------------------------------------------------//=//
	#endregion fields
	/////////////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[MONO]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region mono
	//=//-----|Events|----------------------------------------------------------//=//
	#region events
	protected override void CharacterAwake()
	{

	}
	protected override void CharacterStart()
	{

	}
	#endregion events
	//=//-----|Updates|----------------------------------------------------------//=//
	#region updates 
	protected override void CharacterUpdate()
	{

	}
	protected override void CharacterFixedFrameUpdate()
	{

	}
	protected override void CharacterFixedPhysicsUpdate()
	{

	}
	protected override void CharacterLateUpdate()
	{

	}
	#endregion updates
	//=//------------------------------------------------------------------------//=//
	#endregion mono
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//=//------------------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base 
	//Base methods and their helpers 
	//=//-----|Setup|------------------------------------------------------------//=//
	#region setup
	protected override void CharacterSetup()
	{

	}
	protected override void RegisterCommands()
	{

	}
	protected override void SetMemberVariables()
	{

	}
	protected override void SetReferences()
	{

	}
	#endregion setup
	//=//-----|Data|-------------------------------------------------------------//=//
	#region data
	protected override void ProcessInput()
	{
	}
	protected override void SetCharacterDimensions()
	{

	}
	protected override void UpdateActiveCharacterData()
	{

	}
	protected override void UpdateCharacterData() //TODO: better name 
	{

	}
	#endregion data
	//=//------------------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[DEBUG]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region debug
	//=//-----|General|---------------------------------------------------------//=//
	//=//------------------------------------------------------------------------//=//
	#endregion debug 
	/////////////////////////////////////////////////////////////////////////////////////
}
