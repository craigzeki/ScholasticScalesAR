using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class WeighableObject : MonoBehaviour, iDragAndDrop
{

    [SerializeField] TextMeshPro debugText;


    //[SerializeField] private Collider myScales = null;
    //private GameObject collidingWeighable = null;


    //public Collider MyScales { get => myScales; set => myScales = value; }

    //private bool scalesChanged = false;
    //[SerializeField] private int numOfCollisions = 0;

    [SerializeField] GameObject groundMarkerPrefab;
    [SerializeField] GameObject groundMarker;
    [SerializeField] GameObject cheveronPrefab;
    private GameObject firstCheveron;
    private float cheveronYSize;


    GameObject[] dropPoints;
    float[] distances;
    private GameObject closestDropPoint;
    private Vector2 dragStartPos2D;
    private float dragStartPosY;


    //Refactor vars
    [SerializeField] private float myMass;
    private Rigidbody myRB;
    private GameObject _directBucket = null;
    private GameObject _byRefBucket = null;
    private GameObject _bucket = null;
    private Dictionary<int, GameObject> touchingWeighables = new Dictionary<int, GameObject>();

    public float MyMass { get => myMass; }
    public GameObject Bucket { get => _bucket;}

    private void Awake()
    {
        myRB = GetComponent<Rigidbody>();
        Debug.Assert(myRB != null, "Weighable:Awake:Assert myRB cannot be null");
        Debug.Assert(groundMarkerPrefab != null, "Weighable:Awake:Assert groundMarkerPrefab cannot be null");
        Debug.Assert(cheveronPrefab != null, "Weighable:Awake:Assert cheveronPrefab cannot be null");

        groundMarker = Instantiate(groundMarkerPrefab);
        groundMarker.SetActive(false);

        firstCheveron = CreateCheveron();
        cheveronYSize = firstCheveron.GetComponent<CheckBounds>().MyBoundsSize.y;
        firstCheveron.SetActive(false);

        myMass = myRB.mass;
    }

    public GameObject CreateCheveron()
    {
        GameObject temp = Instantiate(cheveronPrefab);
        temp.GetComponent<cheveron>().transmittingObject = this.gameObject;
        temp.GetComponent<cheveron>().targetObject = groundMarker;
        temp.transform.position = transform.position;
        temp.GetComponent<cheveron>().SetMyScale();


        return temp;
    }

    // Start is called before the first frame update
    void Start()
    {
        dragStartPosY = transform.position.y;
        
        cheveronYSize = firstCheveron.GetComponent<CheckBounds>().MyBoundsSize.y;
        //cheveronYSize = firstCheveron.transform.localScale.y;
        Destroy(firstCheveron);
    }

    private void OnEnable()
    {

        GameManager.Instance.OnGameStateChanged += GameStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) { return; }
        GameManager.Instance.OnGameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(GameStates previousGameState, GameStates newGameState)
    {
        //guard
        if (newGameState != GameStates.GameSelection) { return; }

        //scales are now in position, set the y position for calculating movement
        dragStartPosY = transform.position.y;
        dragStartPos2D = new Vector2(transform.position.x, transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
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

        

       

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Old code
        //numOfCollisions++;
        //myRB.mass = 0;
        //Debug.Log(this.gameObject.name.ToString() + ": Collision with: " + collision.gameObject.name.ToString());
        //switch (collision.gameObject.tag)
        //{
        //    case "Scales":
        //        scalesChanged = true;
        //        //scales will do the hard part of detecting which collider and grabing the mass from this object
        //        //just keep track of the fact that we are on the scales so that we do not pass our mass to other weighableobjects
        //        break;

        //    case "WeighableObject":
        //        collidingWeighable = collision.gameObject;
        //        if ((scalesChanged == false) && (myScales == null))
        //        {
        //            myScales = collision.gameObject.GetComponent<WeighableObject>().MyScales;
        //            if (myScales != null)
        //            {
        //                myScales.GetComponentInParent<Scales>().addMass(myMass, myScales);
        //                //scalesChanged = true;
        //            }

        //        }
        //        break;
        //}
    }

    private void OnCollisionExit(Collision collision)
    {

        //if ((--numOfCollisions == 0) || (collision.gameObject.tag == "Scales"))
        //{
        //    myRB.mass = MyMass;
        //    if (myScales != null)
        //    {
        //        myScales.GetComponentInParent<Scales>().removeMass(myMass, myScales);
        //        myScales = null;
        //        scalesChanged = true;
        //    }

        //}


    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Ground":
                //reset the start pos
                dragStartPosY = transform.position.y;
                dragStartPos2D = new Vector2(transform.position.x, transform.position.z);
                break;
            case "Bucket":
                _directBucket = other.gameObject;
                CheckAndSetBucket();
                break;
            case "WeighableObject":
                touchingWeighables[other.gameObject.GetInstanceID()] = other.gameObject.GetComponent<WeighableObject>().Bucket;
                CheckAndSetBucket();
                break;
        }

        
    }

    public bool CheckAndSetBucket()
    {
        if (_directBucket != null)
        {
            //use these
            _bucket = _directBucket;
        }
        else if (touchingWeighables.Count > 0)
        {
            _byRefBucket = null;
            //backups
            foreach (KeyValuePair<int, GameObject> bucket in touchingWeighables)
            {
                if (bucket.Value != null)
                {
                    _byRefBucket = bucket.Value;
                    
                    break;
                }
            }
            _bucket = _byRefBucket;
        }
        else
        {
            _bucket = null;
        }

        if (_bucket != null)
        {
            myRB.mass = 0.01f;
            _bucket.GetComponent<Bucket>().addMass(this.gameObject, MyMass);
            return true;
        }
        else
        {
            myRB.mass = MyMass;
            return false;
        }
    }

    public bool CheckAndSetBucket(GameObject exitingColliderGameObject)
    {
        if(!CheckAndSetBucket())
        {
            exitingColliderGameObject.GetComponent<Bucket>().removeMass(this.gameObject);
            return false;
        }
        else
        {
            return true;
        }
    }



    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Bucket":
                _directBucket = null;
                CheckAndSetBucket(other.gameObject);
                break;
            case "WeighableObject":
                touchingWeighables.Remove(other.gameObject.GetInstanceID());
                CheckAndSetBucket(other.gameObject.GetComponent<WeighableObject>().Bucket);
                break;
        }

        
    }

    public void OnStartDrag()
    {
        myRB.useGravity = false;
        myRB.freezeRotation = true;
        closestDropPoint = GetClosestDropPoint();
        //dragStartPos2D = new Vector2(transform.position.x, transform.position.z);
        //dragStartPosY = transform.position.y;

        groundMarker.SetActive(true);
        groundMarker.transform.localScale = new Vector3(transform.lossyScale.x, transform.lossyScale.y * 0.2f, transform.lossyScale.z);

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

        LayerMask myMask = 1 << LayerMask.NameToLayer("Cheveron");
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
            if(cheveronHit.distance > (cheveronYSize/3))
            {
                CreateCheveron();
            }
        }
        else
        {
            if ((hit.distance > cheveronYSize/3))
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
        debugText.text = "DropPoint y: " + closestDropPoint.transform.position.y.ToString();
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
}
