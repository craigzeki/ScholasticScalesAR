//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using ZekstersLab.Helpers;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CheckBounds))]
public class WeighableObject : MonoBehaviour, iDragAndDrop
{
    //Debug
    //[SerializeField] TextMeshPro debugText;
    //End Debug


    [SerializeField] GameObject groundMarkerPrefab;
    [SerializeField] GameObject groundMarker;
    [SerializeField] GameObject cheveronPrefab;
    [SerializeField] Transform cheveronSpawnPoint;
    private GameObject firstCheveron;
    private FixRotationToZero myFixedRotator;
    private float cheveronYSize;

    private bool isGrounded = false;

    GameObject[] dropPoints;
    float[] distances;
    private GameObject closestDropPoint;
    private Vector2 dragStartPos2D;
    private float dragStartPosY;


    //Refactor vars
    [SerializeField] private float myMass;
    private Rigidbody myRB;
    private GameObject directBucket = null;
    private GameObject byRefBucket = null;
    private GameObject bucket = null;
    private Dictionary<int, GameObject> touchingWeighables = new Dictionary<int, GameObject>();

    public float MyMass { get => myMass; }
    public GameObject Bucket { get => bucket;}
    public bool IsGrounded { get => isGrounded;}
    int gap1 = 3;
    int gap2 = 1;
    private void Awake()
    {
        //Setup references to components
        myRB = GetComponent<Rigidbody>();
        TryGetComponent<FixRotationToZero>(out myFixedRotator);

        //guards and asserts
        Debug.Assert(myRB != null, "Weighable:Awake:Assert myRB cannot be null");
        Debug.Assert(groundMarkerPrefab != null, "Weighable:Awake:Assert groundMarkerPrefab cannot be null");
        Debug.Assert(cheveronPrefab != null, "Weighable:Awake:Assert cheveronPrefab cannot be null");
        Debug.Assert(cheveronSpawnPoint != null, "Weighable:Awake:Assert cheveronSpawnPoint cannot be null");

        //assume we are on the ground for first start
        //could be replaced with a mechanism to check this first
        setIsGrounded(false);

        //create the ground marker and hide it for now (shown while moving the object around)
        groundMarker = Instantiate(groundMarkerPrefab);
        groundMarker.SetActive(false);

        //Cheveron template created here and then used to get theY size for later use and scaling
        //Getting the ySize is done in Start() as this requires the CheckBounds script attached to the Cheveron prefab
        //to be available and have executed once
        firstCheveron = CreateCheveron();
        firstCheveron.SetActive(false);

        //Set this objects mass to the one configured in the rigid body
        myMass = myRB.mass;
    }

    //things done here cannot be done in awake as they require the object to finish instantiating and to populate data into properties so that it can be used here
    void Start()
    {
        //set the initial y position to be used in the drag / drop mechanism
        dragStartPosY = transform.position.y;

        //get the cheveron y size for use in scaling calulations later on
        //can remove the template cheveron then as no longer needed
        cheveronYSize = firstCheveron.GetComponent<CheckBounds>().MyBoundsSize.y;
        Destroy(firstCheveron);
    }

    private void OnEnable()
    {
        //subscribe to the game manager OnGameStateChanged
        GameManager.Instance.OnGameStateChanged += GameStateChanged;
    }

    private void OnDisable()
    {
        //unsubscribe from the game manager OnGameStateChanged, providing the game manager hasn't already been destroyed
        if (GameManager.Instance == null) { return; }
        GameManager.Instance.OnGameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(GameStates previousGameState, GameStates newGameState)
    {
        //This gets fired when the Game Manager changes state
        //Used here to ensure we reset the drag drop touch parameters for calculating height

        //guard - must be in GameSelection mode
        if (newGameState != GameStates.GameSelection) { return; }

        //scales are now in position, set the y position for calculating movement
        dragStartPosY = transform.position.y;
        dragStartPos2D = new Vector2(transform.position.x, transform.position.z);

    }


    public GameObject CreateCheveron()
    {
        //This function will create a single cheveron and scale it based on the object it is being transmitted from

        GameObject temp = Instantiate(cheveronPrefab);
        temp.GetComponent<cheveron>().transmittingObject = this.gameObject;
        temp.GetComponent<cheveron>().targetObject = groundMarker;
        temp.transform.position = cheveronSpawnPoint.position;

        //calculate the scale to make the cheveron match 75% of this weighable objects scale
        Helpers.matchXSizeLockedAspectRatio(this.gameObject, temp, 0.75f);

        //force a recalculate of the cheveron bounds
        temp.GetComponent<CheckBounds>().calcBoundsNow();

        return temp;
    }

    private void setIsGrounded(bool state)
    {
        //Set is grounded state and update the myFixedRotator state if it is attached
        isGrounded = state;
        if(myFixedRotator == null) { return; }
        myFixedRotator.enabled = isGrounded;
    }
   
    private void OnTriggerEnter(Collider other)
    {
        //handle the various things we can trigger / be triggered by
        switch (other.tag)
        {
            case "Ground":
                //reset the start pos for drag / drop
                dragStartPosY = transform.position.y;
                dragStartPos2D = new Vector2(transform.position.x, transform.position.z);
                setIsGrounded(true);
                break;
            case "Bucket":
                //store the reference to the bucket we are touching
                directBucket = other.gameObject;
                //set this as my bucket - will override any other buckets that are set indirectly (if this was only previously touching a weighable object)
                CheckAndSetBucket();
                break;
            case "WeighableObject":
                //touching a weighable object
                //get the referebce to this weighables bucket (if it has one) and store it in the dictionary
                touchingWeighables[other.gameObject.GetInstanceID()] = other.gameObject.GetComponent<WeighableObject>().Bucket;
                //if there is no 'direct bucket' that we are touching then set the indirect bucket from the other weighable as ours for now
                CheckAndSetBucket();
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Ground":
                //set the start pos for drag / drop
                dragStartPosY = transform.position.y;
                dragStartPos2D = new Vector2(transform.position.x, transform.position.z);
                setIsGrounded(false);
                break;
            case "Bucket":
                //clear the direct bucket as no longer touching one
                directBucket = null;
                //revert to indirect bucket or no bucket
                CheckAndSetBucket(other.gameObject);
                break;
            case "WeighableObject":
                //remove the weighable we were touching from the list of weighables
                touchingWeighables.Remove(other.gameObject.GetInstanceID());
                //recheck if we have any bucket and if not, remove the mass from the previously linked bucket
                CheckAndSetBucket(other.gameObject.GetComponent<WeighableObject>().Bucket);
                break;
        }


    }

    private void OnTriggerStay(Collider other)
    {
        //used to enforce isGrounded
        switch (other.tag)
        {
            case "Ground":

                setIsGrounded(true);
                break;
            
        }
    }

    public bool CheckAndSetBucket()
    {
        //This function checks to see which buckets, indirect or direct are available and
        //sets the one with highest priority (direct has highest)

        if (directBucket != null)
        {
            //highest priority - do this check first and exit function ASAP
            bucket = directBucket;
        }
        else if (touchingWeighables.Count > 0)
        {
            //check for a bucket in the weighables that we are touching
            //as each bucket is far apart, we will assume that all weighables in
            //this list will contain the same bucket, and therefore can exit
            //the ForEach loop as soon as one is found
            byRefBucket = null;
            //backups
            foreach (KeyValuePair<int, GameObject> bucket in touchingWeighables)
            {
                if (bucket.Value != null)
                {
                    byRefBucket = bucket.Value;
                    
                    break;
                }
            }
            bucket = byRefBucket;
        }
        else
        {
            //no bucket available
            bucket = null;
        }

        //if we have a bucket, set this objects weight to 0 (or as close as we can get within Unity physics)
        //Add the mass to the bucket
        if (bucket != null)
        {
            myRB.mass = 0.01f;
            bucket.GetComponent<Bucket>().addMass(this.gameObject, MyMass);
            return true;
        }
        else
        {
            //we are no longer connected to a bucket, restore the mass of the object
            myRB.mass = MyMass;
            return false;
        }
    }

    public bool CheckAndSetBucket(GameObject exitingColliderGameObject)
    {
        //This Method Overload provides a way to indicate that the function was called from the 'OnTriggerExit' function
        //It takes an additinal member which we can use to remove our weight from the bucket
        if(!CheckAndSetBucket())
        {
            //call the base function first and if no other buckets exist...
            if (exitingColliderGameObject != null)
            {
                //if the member is set, get its bucket and remove our mass
                exitingColliderGameObject.GetComponent<Bucket>().removeMass(this.gameObject);
            }
            
            //return false
            return false;
        }
        else
        {
            //return true as the base function would have also returned true in this case
            return true;
        }
    }


    //Touch Controls and associated (markers & cheverons) below this point

    public void OnStartDrag()
    {
        myRB.useGravity = false;
        myRB.freezeRotation = true;
        closestDropPoint = GetClosestDropPoint();
        //dragStartPos2D = new Vector2(transform.position.x, transform.position.z);
        //dragStartPosY = transform.position.y;

        groundMarker.SetActive(true);
        Helpers.matchXSizeLockedAspectRatio(this.gameObject, groundMarker, 0.75f);

        drawGroundMarker();

    }

    public void OnEndDrag()
    {
        myRB.useGravity = true;
        myRB.freezeRotation = false;
        myRB.velocity = Vector3.zero;
        closestDropPoint = null;
        //dragStartPos2D = Vector2.zero;
        //dragStartPosY = 0f;

        groundMarker.SetActive(false);
    }

    static List<RaycastHit> hits = new List<RaycastHit>();

    private void drawGroundMarker()
    {
        //LineRenderer myLine = new LineRenderer();

        Ray ray = new Ray(transform.position, Vector3.down);

        int myMask = 1 << LayerMask.NameToLayer("Cheveron");
        myMask = ~myMask;

        //hits.AddRange(Physics.RaycastAll(ray, Mathf.Infinity, myMask));

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, myMask))
        {
            groundMarker.transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        //if (hits.Count > 0)
        //{
        //    hits.Sort((h1, h2) => h1.distance.CompareTo(h2.distance));


        //    groundMarker.transform.position = new Vector3(transform.position.x, hits[0].point.y, transform.position.z);

        //}




        if (Physics.Raycast(ray, out RaycastHit cheveronHit, Mathf.Infinity, ~myMask))
        {
            if(cheveronHit.distance > (cheveronYSize*gap1))
            {
                CreateCheveron();
            }
        }
        else
        {
            if ((hit.distance > cheveronYSize*gap2))
            {
                CreateCheveron();
            }
        }

    }

    public void AfterDragPosChange()
    {
        gameObject.TryGetComponent<Rigidbody>(out Rigidbody myRB);


        //check if closest point has changed
        GameObject tempClosestPoint = GetClosestDropPoint();
        if (tempClosestPoint != closestDropPoint)
        {
            closestDropPoint = tempClosestPoint;
            //dragStartPos2D = new Vector2(transform.position.x, transform.position.z);
            //dragStartPosY = transform.
        }
        Vector2 dropPoint2DPos = new Vector2(closestDropPoint.transform.position.x, closestDropPoint.transform.position.z);
        Vector2 myCurrent2DPos = new Vector2(transform.position.x, transform.position.z);
        //debugText.text = "DropPoint y: " + closestDropPoint.transform.position.y.ToString();
        // y = mx+c
        // c = y-intersect = the start y pos of the weighable object
        // m = gradient (delta y / delta x) between the (dropPoint y - start weighable y) / distance between droppoint2D and start weighable2D
        // x = the current distance between the now and the startpos
        // y can then be calculated.

        float c = dragStartPosY;
        //float m = (closestDropPoint.transform.position.y - dragStartPosY) / (Vector2.Distance(dragStartPos2D, dropPoint2DPos));
        float m = 0;
        if (dragStartPos2D.y < dropPoint2DPos.y)
        {
            m = (closestDropPoint.transform.position.y - dragStartPosY) / (dropPoint2DPos.y - dragStartPos2D.y);
        }
        else
        {
            m = (closestDropPoint.transform.position.y - dragStartPosY) / (dragStartPos2D.y - dropPoint2DPos.y);
        }

        //float x = Vector2.Distance(myCurrent2DPos, dragStartPos2D);
        float x = 0;
        //calculate x pos correctly depending on if we are above
        //or below the drop point
        if (dragStartPos2D.y < dropPoint2DPos.y)
        {

            x = myCurrent2DPos.y - dragStartPos2D.y;
        }
        else
        {
            x = dragStartPos2D.y - myCurrent2DPos.y;
        }


        float y = Mathf.Clamp((m * x) + c, dragStartPosY, closestDropPoint.transform.position.y);

        //turn distance into velocity to be added to the already calculated x and z velocity in the DragAndDrop manager
        //float velocityY = (y - transform.position.y) * Time.deltaTime * 40;
        //myRB.velocity = new Vector3(myRB.velocity.x, velocityY, myRB.velocity.z);
        myRB.position = new Vector3(myRB.position.x, y, myRB.position.z);

        //show marker on ground for better location understanding
        drawGroundMarker();

    }

    private GameObject GetClosestDropPoint()
    {
        //calculate how close to the scales and adjust y pos to lift object the closer it is
        GameObject scales = GameManager.Instance.TheScales;
        Debug.Assert(scales != null, "WeighableObject:AfterDragPosChange: scales cannot be null");

        dropPoints = GameObject.FindGameObjectsWithTag("DropPoint");
        if (dropPoints.Length == 0) { return null; }
        distances = new float[dropPoints.Length];
        for (int i = 0; i < dropPoints.Length; i++)
        {
            distances[i] = Vector3.Distance(transform.position, dropPoints[i].transform.position);
        }

        //sort the dropPoints by distance
        System.Array.Sort(distances, dropPoints);

        return dropPoints[0];
    }


    //Old code - left here so it can be seen what did not work!

    // Update is called once per frame
    //void Update()
    //{
        //Old code
        //if(scalesChanged)
        //{

        //    if(myScales != null)
        //    {
        //        Debug.Log(this.gameObject.name.ToString() + ": Scales Changed: " + myScales.name.ToString());
        //        myScales.GetComponentInParent<Scales>().addMass(myMass, myScales);
        //    }
        //    else { Debug.Log(this.gameObject.name.ToString() + "Scales Null"); }

        //    scalesChanged = false;
        //}

        //Refactor code

    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Old code
    //    //numOfCollisions++;
    //    //myRB.mass = 0;
    //    //Debug.Log(this.gameObject.name.ToString() + ": Collision with: " + collision.gameObject.name.ToString());
    //    //switch (collision.gameObject.tag)
    //    //{
    //    //    case "Scales":
    //    //        scalesChanged = true;
    //    //        //scales will do the hard part of detecting which collider and grabing the mass from this object
    //    //        //just keep track of the fact that we are on the scales so that we do not pass our mass to other weighableobjects
    //    //        break;

    //    //    case "WeighableObject":
    //    //        collidingWeighable = collision.gameObject;
    //    //        if ((scalesChanged == false) && (myScales == null))
    //    //        {
    //    //            myScales = collision.gameObject.GetComponent<WeighableObject>().MyScales;
    //    //            if (myScales != null)
    //    //            {
    //    //                myScales.GetComponentInParent<Scales>().addMass(myMass, myScales);
    //    //                //scalesChanged = true;
    //    //            }

    //    //        }
    //    //        break;
    //    //}
    //}

    //private void OnCollisionExit(Collision collision)
    //{

    //    //if ((--numOfCollisions == 0) || (collision.gameObject.tag == "Scales"))
    //    //{
    //    //    myRB.mass = MyMass;
    //    //    if (myScales != null)
    //    //    {
    //    //        myScales.GetComponentInParent<Scales>().removeMass(myMass, myScales);
    //    //        myScales = null;
    //    //        scalesChanged = true;
    //    //    }

    //    //}

    //}
}
