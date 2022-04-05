//Author: Craig Zeki
//Student ID: zek21003166

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
    WeightSelection,
    InstallNewAddOn,
    LearnMode,
    QuizMode,
    NumOfStates
}

public enum GameTypes : int
{
    Learn = 0,
    Quiz,
    NumOfTypes
}

public enum WeightType : int
{
    CartoonBlocks = 0,
    FarmAnimals,
    Fruits,
    NumofWeights
}

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public delegate void gameStateChanged(GameStates previousState, GameStates newState);
    public event gameStateChanged OnGameStateChanged;

    private GameStates currentGameState;
    private GameStates newGameState;
    private GameTypes gameType = GameTypes.NumOfTypes;

    private static GameManager instance;

    private GameObject theScales;

    [SerializeField] private Canvas selectWeightsCanvas;
    [SerializeField] private Canvas selectGameTypeCanvas;

    private Transform leftSpawnPoint = null;
    private Transform rightSpawnPoint = null;

    [SerializeField] private GameObject quizBlockPrefab;
    [SerializeField] private GameObject learnBlockPrefab;
    [SerializeField] private GameObject cartoonBlocks1Prefab;
    [SerializeField] private GameObject cartoonBlocks2Prefab;
    [SerializeField] private GameObject animalBlocks1Prefab;
    [SerializeField] private GameObject animalBlocks2Prefab;

    private AudioSource audioSource;

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
    public GameTypes GameType { get => gameType; set => gameType = value; }

    private void Awake()
    {
        //Check gaurds


        //init the canvas
        selectGameTypeCanvas.enabled = false;
        selectWeightsCanvas.enabled = false;

        
        //initialise the state machines
        currentGameState = GameStates.Initialised;
        NewGameState = GameStates.Initialised;

        //get the audio source (required component)
        audioSource = this.gameObject.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    void Update()
    {
        //handle game state machine cyclic code
        switch (currentGameState)
        {
            case GameStates.Initialised:
                NewGameState = GameStates.SetupPlayArea;
                break;
            case GameStates.SetupPlayArea:
                break;
            case GameStates.GameSelection:
                if(GameType != GameTypes.NumOfTypes)
                {
                    //user has selected a game
                    NewGameState = GameStates.WeightSelection;
                }
                break;
            case GameStates.WeightSelection:
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


    private void SetCurrentGameState(GameStates newState)
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

                    SetCurrentGameState(NewGameState);
                }
                break;
            case GameStates.SetupPlayArea:
                if(currentGameState == GameStates.Initialised)
                {
                    //when entering from Initialised
                    SetCurrentGameState(NewGameState);
                }
                break;
            case GameStates.GameSelection:
                if(currentGameState == GameStates.SetupPlayArea)
                {
                    selectGameTypeCanvas.enabled = true;
                    addWeight(quizBlockPrefab, leftSpawnPoint);
                    addWeight(learnBlockPrefab, rightSpawnPoint);


                    SetCurrentGameState(NewGameState);
                }
                break;
            case GameStates.WeightSelection:
                if(currentGameState == GameStates.GameSelection)
                {
                    cleanUpScene();
                    selectGameTypeCanvas.enabled = false;
                    selectWeightsCanvas.enabled = true;

                    SetCurrentGameState(NewGameState);
                }
                break;
            case GameStates.InstallNewAddOn:
                break;
            case GameStates.LearnMode:
                if(currentGameState == GameStates.WeightSelection)
                {
                    selectWeightsCanvas.enabled = false;

                    SetCurrentGameState(NewGameState);
                }
                break;
            case GameStates.QuizMode:
                if (currentGameState == GameStates.WeightSelection)
                {
                    selectWeightsCanvas.enabled = false;

                    SetCurrentGameState(NewGameState);
                }
                break;
            case GameStates.NumOfStates:
                break;
            default:
                break;
        }
    }

    private List<GameObject> weightsInScene = new List<GameObject>();

    private void addWeight(GameObject weight, Transform spawnPoint)
    {
        GameObject temp = Instantiate(weight, TheScales.transform);
        weightsInScene.Add(temp);
        temp.transform.localPosition = spawnPoint.localPosition;
    }

    private void cleanUpScene()
    {
        foreach(GameObject weight in weightsInScene)
        {
            Destroy(weight);
        }
        weightsInScene.Clear();
        theScales.GetComponent<Scales>().resetScaleWeights();
    }

    public void SetupPlayAreaComplete(GameObject scales)
    {
        theScales = scales;
        leftSpawnPoint = theScales.GetComponent<Scales>().leftSpawnPoint;
        rightSpawnPoint = theScales.GetComponent<Scales>().rightSpawnPoint;
        NewGameState = GameStates.GameSelection;
    }

    public void playSound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void FarmAnimalsSelected(AudioClip audioClip)
    {
        addWeight(animalBlocks1Prefab, leftSpawnPoint);
        addWeight(animalBlocks2Prefab, rightSpawnPoint);
        WeightsSelected(audioClip);
    }

    public void CartoonBlocksSelected(AudioClip audioClip)
    {
        addWeight(cartoonBlocks1Prefab, leftSpawnPoint);
        addWeight(cartoonBlocks2Prefab, rightSpawnPoint);
        WeightsSelected(audioClip);
    }

    public void FruitsSelected()
    {

    }

    private void WeightsSelected(AudioClip audioClip)
    {
        //detect which pack is selected and load
        audioSource.clip = audioClip;
        audioSource.Play();


        //switch to already selected game state
        switch (gameType)
        {
            case GameTypes.Learn:
                NewGameState = GameStates.LearnMode;
                break;
            case GameTypes.Quiz:
                NewGameState = GameStates.QuizMode;
                break;
            case GameTypes.NumOfTypes:
                break;
            default:
                break;
        }
    }
}
