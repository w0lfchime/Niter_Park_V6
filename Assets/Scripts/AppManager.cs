using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }



    public List<Player> Players { get; private set; }
    public AppState CurrentState { get; private set; }


    public string DevLayerScene = "DevLayer";


    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

       






    }








    // 
    void SetAppState(AppState state)
    {
        // Exit the current state if it exists
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }

        // Assign the new state
        CurrentState = state;

        // Start the new state
        if (CurrentState != null)
        {
            CurrentState.Enter();
        }
    }

    //Mono Behaviour
    void Update()
    {
        // Periodically update settings
        if (Input.GetKeyDown(KeyCode.U)) // For testing: manually trigger updates
        {
            //globalSettings.UpdateSettings();
        }


        
    }

    private void FixedUpdate()
    {
        // Update global data, such as screen size 
        //GlobalDataUpdates();
        //CurrentState.FixedUpdate();
    }




    private void GlobalDataUpdates()
    {
        //globalSettings.UpdateSettings();
    }
}
