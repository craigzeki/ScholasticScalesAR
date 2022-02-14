using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public enum GameStates : int
{
    Initialised = 0,
    SetupPlayArea,
    GameSelection,
    InstallNewAddOn,
    LearnMode,
    QuizMode,
    NumOfStates
}

public class GameManager : MonoBehaviour
{
    public delegate void gameStateChanged(GameStates previousState, GameStates newState);
    public event gameStateChanged OnGameStateChanged;

    private GameStates currentGameState;
    private GameStates newGameState;

    private static GameManager instance;

    private GameObject theScales;

    public GameStates NewGameState
    {
        get => newGameState;
        
        set
        {
            newGameState = value;
            StartStateTransition();
        }
    }

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
                //Debug.Assert(instance != null, "GameManager:Instance: instance cannot be null - could not find GameManager instance");
            }
            return instance;
        }
    }

    public GameStates CurrentGameState { get => currentGameState; }
    public GameObject TheScales { get => theScales;  }

    private void Awake()
    {
        //Check gaurds

        Debug.Log("Debug");
        
        //initialise the state machines
        currentGameState = GameStates.Initialised;
        NewGameState = GameStates.Initialised;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentGameState)
        {
            case GameStates.Initialised:
                NewGameState = GameStates.SetupPlayArea;
                break;
            case GameStates.SetupPlayArea:
                break;
            case GameStates.GameSelection:
                Debug.Log("Test");
                break;
            case GameStates.InstallNewAddOn:
                break;
            case GameStates.LearnMode:
                break;
            case GameStates.QuizMode:
                break;
            case GameStates.NumOfStates:
                break;
            default:
                break;
        }
    }


    private void setCurrentGameState(GameStates newState)
    {
        if (OnGameStateChanged != null) OnGameStateChanged(currentGameState, newState);
        currentGameState = newState;
    }

    private void StartStateTransition()
    {
        switch (NewGameState)
        {
            case GameStates.Initialised:
                if(currentGameState == GameStates.Initialised)
                {
                    // only do this when starting the game - both states must be initialised

                    setCurrentGameState(NewGameState);
                }
                break;
            case GameStates.SetupPlayArea:
                if(currentGameState == GameStates.Initialised)
                {
                    //when entering from Initialised
                    setCurrentGameState(NewGameState);
                }
                break;
            case GameStates.GameSelection:
                if(currentGameState == GameStates.SetupPlayArea)
                {


                    setCurrentGameState(NewGameState);
                }
                break;
            case GameStates.InstallNewAddOn:
                break;
            case GameStates.LearnMode:
                break;
            case GameStates.QuizMode:
                break;
            case GameStates.NumOfStates:
                break;
            default:
                break;
        }
    }

    public void SetupPlayAreaComplete(GameObject scales)
    {
        theScales = scales;
        NewGameState = GameStates.GameSelection;
    }
}
