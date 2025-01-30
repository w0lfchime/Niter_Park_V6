using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }



    public HashSet<Player> players { get; private set; }
    public AppState CurrentState { get; private set; }


    public string DevLayerScene = "DevLayer";


    
    public void AddPlayer(string name)
    {
        players.Add(new Player(name))
    }


    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //dd on load ?
    }
    //TODO: implement commands 
    void RegisterCommands()
    {
        CommandHandler.RegisterCommand("appstate", args =>
        {
            switch (args[1])
            {
                case "set":

                    break;
                case "reset":
                    AppState newState = (AppState)Activator.CreateInstance(CurrentState.GetType());
                    SetAppState(newState);
                    break;
                default:
                    LogCore.Log($"Unknown argument: {args[1]}");
                    break;
            }
        });
    }
    void SetAppState(string appstate)
    {
        switch (appstate)
        {
            case "match":
                //AppState newAppState = new Match();

                //SetAppState(newAppState);
                break;

            default:
                LogCore.Log($"Unknown appstate: {appstate}");
                break;
        }
    }
    void SetAppState(AppState state)
    {
        // Exit the current state if it exists
        if (CurrentState != null)
        {
            CurrentState.Exit();


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




        }

        void FixedUpdate()
        {
            // Update global data, such as screen size 
            //GlobalDataUpdates();
            //CurrentState.FixedUpdate();
        }





    }





}
