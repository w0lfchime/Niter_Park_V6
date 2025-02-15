using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class DebugCore
{


	public static void StopGame()
	{
#if UNITY_EDITOR
    EditorApplication.isPlaying = false; // Stops Play Mode
#else
		Application.Quit(); // Stops built game
#endif
	}

}
