using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }


    public List<Player> Players { get; private set; }
    public AppState CurrentState { get; private set; }


    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // - - - - - - - - - 

        // Dev setup




        // Setup

        InitializeApplication();







    }



    void InitializeApplication()
    {

        AppState homeMenu = new HomeAppState();
    }

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
            CurrentState.Start();
        }
    }




    void Update()
    {
        // Periodically update settings
        if (Input.GetKeyDown(KeyCode.U)) // For testing: manually trigger updates
        {
            globalSettings.UpdateSettings();
        }


        CurrentState.Start();
    }

    private void FixedUpdate()
    {
        // Update global data, such as screen size 
        GlobalDataUpdates();
        CurrentState.FixedUpdate();
    }




    private void GlobalDataUpdates()
    {
        globalSettings.UpdateSettings();
    }
}
