using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public enum ScalesPlacementState
{
    Initialised = 0,
    Scanning,
    Positioning,
    Complete,
    NumOfStates
}

public class ScalesPlacementManager : MonoBehaviour
{
    private ScalesPlacementState currentState;
    private ScalesPlacementState newState;

    private ARPlaneManager myARPlaneManager;
    private List<ARPlane> arPlanes = new List<ARPlane>();

    [SerializeField] GameObject scalesPrefab;
    private CheckBounds scalesBounds;
    private Vector3 scalesSize = Vector3.zero;

    [SerializeField] Material largePlaneMaterial;

    [SerializeField] Canvas SetupPlayAreaCanvas;
    [SerializeField] TextMeshProUGUI scanningText;
    [SerializeField] TextMeshProUGUI positioningText;
    [SerializeField] Button doneButton;
    [SerializeField] Image scanner;

    [SerializeField] ARRaycastManager arRaycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private ARPlane selectedPlane = null;

    private bool touchInProgress = false;
    private Vector2 touchPos = Vector2.zero;
    private float touchTimeDuration = 0;
    private float touchStartTime = 0;

    

    private GameObject theScales;

    public ScalesPlacementState NewState
    {
        get => newState;
        set
        {
            newState = value;
            StartStateTransition();
        }
    }

    private void Awake()
    {
        myARPlaneManager = GameObject.FindObjectOfType<ARPlaneManager>();
        Debug.Assert((scalesPrefab != null), "ScalesPlacementManager:Awake: scalesPrefab cannot be null");
        Debug.Assert((largePlaneMaterial != null), "ScalesPlacementManager:Awake: largePlaneMaterial cannot be null");
        Debug.Assert((positioningText != null), "ScalesPlacementManager:Awake: positioningText cannot be null");
        Debug.Assert((scanningText != null), "ScalesPlacementManager:Awake: scanningText cannot be null");
        Debug.Assert((SetupPlayAreaCanvas != null), "ScalesPlacementManager:Awake: SetupPlayAreaCanvas cannot be null");
        Debug.Assert(myARPlaneManager != null, "ScalesPlacementManager:Awake: myARPlaneManager cannot be null!");

        theScales = Instantiate(scalesPrefab);
        Debug.Assert(theScales.TryGetComponent<CheckBounds>(out scalesBounds), "ScalesPlacementManager:Awake: Could not find CheckBounds on scalesPrefab");

        myARPlaneManager.enabled = false;
        currentState = ScalesPlacementState.Initialised;
        NewState = ScalesPlacementState.Initialised;
        init();
    }

    private void OnEnable()
    {
        myARPlaneManager.planesChanged += OnPlanesChanged;
        TouchInputManager.Instance.OnStartTouch += touchStarted;
        TouchInputManager.Instance.OnEndTouch += touchEnded;
    }

    private void OnDisable()
    {
        myARPlaneManager.planesChanged -= OnPlanesChanged;
        TouchInputManager.Instance.OnStartTouch -= touchStarted;
        TouchInputManager.Instance.OnEndTouch -= touchEnded;
    }

    private void touchStarted(Vector2 position, float time)
    {
        touchInProgress = true;
        touchPos = position;
        touchTimeDuration = 0;
        touchStartTime = time;
    }

    private void touchEnded(Vector2 position, float time)
    {
        touchInProgress = false;
        touchPos = position;
        touchTimeDuration = time - touchStartTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        //instantiate a new scale so that it can be measured for use when detecting correct sized plane

        scalesBounds.calcBoundsNow();
        scalesSize = scalesBounds.MyBoundsSize;
        theScales.SetActive(false);
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs planesChangedArgs)
    {
        if((planesChangedArgs.added != null) && (planesChangedArgs.added.Count > 0))
        {
            arPlanes.AddRange(planesChangedArgs.added);
        }

        foreach (ARPlane thePlane in arPlanes.Where(thePlane => ((thePlane.size.x > scalesSize.x) &&
                                                                    (thePlane.size.y > scalesSize.z))))
        {
            
            thePlane.GetComponent<Renderer>().material = largePlaneMaterial;
            NewState = ScalesPlacementState.Positioning;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //guards
        if (GameManager.Instance.CurrentGameState != GameStates.SetupPlayArea) { return; }
        
        //run the state machine
        switch (currentState)
        {
            case ScalesPlacementState.Initialised:
                //switch to scanning
                NewState = ScalesPlacementState.Scanning;
                break;
            case ScalesPlacementState.Scanning:
                //start plane tracking and filter for large planes
                //state transition handled by OnPlanesChanged
                break;
            case ScalesPlacementState.Positioning:
                //start touch detection

                UnityEngine.XR.ARSubsystems.TrackableType myPlaneMask;
                myPlaneMask = UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds | UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon;

                if (touchInProgress && !TouchInputManager.IsTouchOverUI(touchPos))
                {
                    if(arRaycastManager.Raycast(touchPos, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
                    {
                        foreach(ARRaycastHit hit in hits)
                        {
                            
                            Pose hitPose = hit.pose;
                            ARPlane hitPlane = myARPlaneManager.GetPlane(hit.trackableId);

                            //make sure we hit a real plane (not an estimated one)
                            if (((hit.hitType & myPlaneMask) == myPlaneMask) && ((hitPlane.size.x > scalesSize.x) &&
                                                                    (hitPlane.size.y > scalesSize.z)))
                            {
                                foreach (ARPlane plane in myARPlaneManager.trackables)
                                {
                                    if (plane != myARPlaneManager.GetPlane(hit.trackableId))
                                    {
                                        plane.gameObject.SetActive(false);
                                    }
                                }

                                selectedPlane = hitPlane;
                                myARPlaneManager.enabled = false;

                                theScales.transform.position = hitPose.position;
                                theScales.TryGetComponent<KeepUpright>(out KeepUpright upright);
                                if(upright != null) { upright.lookAtCamera(); }
                                theScales.SetActive(true);
                                
                                doneButton.interactable = true;
                                break;
                            }

                            
                        }
                        

                    }
                }

                break;
            case ScalesPlacementState.Complete:
                break;
            case ScalesPlacementState.NumOfStates:
                break;
            default:
                break;
        }
    }

    private void init()
    {
        myARPlaneManager.enabled = false;
        scanningText.enabled = false;
        positioningText.enabled = false;
        
        doneButton.interactable = false;
        SetupPlayAreaCanvas.enabled = false;
    }

    private void StartStateTransition()
    {
        //guards
        if (GameManager.Instance.CurrentGameState != GameStates.SetupPlayArea) { return; }

        //run the state transitions
        switch (newState)
        {
            case ScalesPlacementState.Initialised:
                if(currentState == ScalesPlacementState.Initialised)
                {
                    //Only do this if we are just initialized / re-initialized
                    //reset all parameters
                    init();
                    currentState = NewState;
                }
                break;
            case ScalesPlacementState.Scanning:
                if(currentState == ScalesPlacementState.Initialised)
                {
                    //coming here for the first time since initialized
                    
                    //show canvas and scanning text
                    SetupPlayAreaCanvas.enabled = true;
                    scanningText.enabled = true;
                    positioningText.enabled = false;

                    //start scanning for planers
                    myARPlaneManager.enabled = true;
                    currentState = NewState;
                }
                break;
            case ScalesPlacementState.Positioning:
                if ((currentState == ScalesPlacementState.Scanning) || (currentState == ScalesPlacementState.Complete))
                {
                    //coming here from scanning or complete (user wants to reposition))
                    //stop scanner
                    scanner.enabled = false;


                    //show positioning text
                    positioningText.enabled = true;
                    scanningText.enabled = false;
                    currentState = NewState;
                    
                }
                break;
            case ScalesPlacementState.Complete:
                if(currentState == ScalesPlacementState.Positioning)
                {
                    //coming from positioning - scales are now setup

                    //hide the canvas
                    SetupPlayAreaCanvas.enabled = false;
                    GameManager.Instance.SetupPlayAreaComplete(theScales);
                    currentState = NewState;
                }
                break;
            case ScalesPlacementState.NumOfStates:
                break;
            default:
                break;
        }
    }

    public void doneButtonClicked()
    {
        selectedPlane.gameObject.SetActive(false);
        NewState = ScalesPlacementState.Complete;
    }
}
