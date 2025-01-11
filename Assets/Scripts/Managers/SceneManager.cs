using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    // Tracks loaded scenes
    private List<string> loadedScenes = new List<string>();

    /// <summary>
    /// Loads one or more scenes additively.
    /// </summary>
    /// <param name="scenes">Array of scene names to load.</param>
    /// <param name="onComplete">Callback after all scenes are loaded.</param>
    public void LoadScenes(string[] scenes, System.Action onComplete = null)
    {
        StartCoroutine(LoadScenesCoroutine(scenes, onComplete));
    }

    /// <summary>
    /// Unloads one or more scenes.
    /// </summary>
    /// <param name="scenes">Array of scene names to unload.</param>
    /// <param name="onComplete">Callback after all scenes are unloaded.</param>
    public void UnloadScenes(string[] scenes, System.Action onComplete = null)
    {
        StartCoroutine(UnloadScenesCoroutine(scenes, onComplete));
    }

    /// <summary>
    /// Unloads all currently loaded scenes except the persistent scene.
    /// </summary>
    /// <param name="onComplete">Callback after all scenes are unloaded.</param>
    public void UnloadAllScenes(System.Action onComplete = null)
    {
        StartCoroutine(UnloadAllScenesCoroutine(onComplete));
    }

    /// <summary>
    /// Coroutine to load scenes additively.
    /// </summary>
    private IEnumerator LoadScenesCoroutine(string[] scenes, System.Action onComplete)
    {
        foreach (var scene in scenes)
        {
            if (!loadedScenes.Contains(scene))
            {
                var asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
                loadedScenes.Add(scene);
                Debug.Log($"Scene loaded: {scene}");
            }
            else
            {
                Debug.LogWarning($"Scene already loaded: {scene}");
            }
        }

        onComplete?.Invoke();
    }

    /// <summary>
    /// Coroutine to unload scenes.
    /// </summary>
    private IEnumerator UnloadScenesCoroutine(string[] scenes, System.Action onComplete)
    {
        foreach (var scene in scenes)
        {
            if (loadedScenes.Contains(scene))
            {
                var asyncUnload = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
                loadedScenes.Remove(scene);
                Debug.Log($"Scene unloaded: {scene}");
            }
            else
            {
                Debug.LogWarning($"Scene not loaded: {scene}");
            }
        }

        onComplete?.Invoke();
    }

    /// <summary>
    /// Coroutine to unload all scenes except persistent ones.
    /// </summary>
    private IEnumerator UnloadAllScenesCoroutine(System.Action onComplete)
    {
        var scenesToUnload = new List<string>(loadedScenes);
        foreach (var scene in scenesToUnload)
        {
            var asyncUnload = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
            Debug.Log($"Scene unloaded: {scene}");
        }
        loadedScenes.Clear();

        onComplete?.Invoke();
    }
}
