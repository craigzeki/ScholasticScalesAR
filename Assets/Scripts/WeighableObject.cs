using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class WeighableObject : MonoBehaviour, iDragAndDrop
{


    [SerializeField] private float myMass;
    private Rigidbody myRB;

    [SerializeField] private Collider myScales = null;
    private GameObject collidingWeighable = null;

    public float MyMass { get => myMass; }
    public Collider MyScales { get => myScales; set => myScales = value; }

    private bool scalesChanged = false;
    [SerializeField] private int numOfCollisions = 0;

    GameObject[] dropPoints;
    float[] distances;
    private GameObject closestDropPoint;
    private Vector2 dragStartPos2D;
    private float dragStartPosY;

    private void Awake()
    {
        myRB = GetComponent<Rigidbody>();
        Debug.Assert(myRB != null, "Weighable:Awake:Assert myRB cannot be null");
        myMass = myRB.mass;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        if(scalesChanged)
        {
            
            if(myScales != null)
            {
                Debug.Log(this.gameObject.name.ToString() + ": Scales Changed: " + myScales.name.ToString());
                myScales.GetComponentInParent<Scales>().addMass(myMass, myScales);
            }
            else { Debug.Log(this.gameObject.name.ToString() + "Scales Null"); }
            
            scalesChanged = false;
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        numOfCollisions++;
        myRB.mass = 0;
        Debug.Log(this.gameObject.name.ToString() + ": Collision with: " + collision.gameObject.name.ToString());
        switch (collision.gameObject.tag)
        {
            case "Scales":
                scalesChanged = true;
                //scales will do the hard part of detecting which collider and grabing the mass from this object
                //just keep track of the fact that we are on the scales so that we do not pass our mass to other weighableobjects
                break;

            case "WeighableObject":
                collidingWeighable = collision.gameObject;
                if ((scalesChanged == false) && (myScales == null))
                {
                    myScales = collision.gameObject.GetComponent<WeighableObject>().MyScales;
                    if (myScales != null)
                    {
                        myScales.GetComponentInParent<Scales>().addMass(myMass, myScales);
                        //scalesChanged = true;
                    }

                }
                break;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        
        /*
        //if touching something, check if that something is connected to the scales, if yes, add this objects weight to the scales also
 

        switch(collision.gameObject.tag)
        {
            case "Scales":
                myScales = collision.gameObject;
                scalesChanged = true;
                break;

            case "WeighableObject":
                collidingWeighable = collision.gameObject;
                if (myScales == null)
                {
                    myScales = collision.gameObject.GetComponent<Weighable>().MyScales;
                    if(myScales != null) { scalesChanged = true; }

                }
                break;
        }*/
    }

    private void OnCollisionExit(Collision collision)
    {
        
        if((--numOfCollisions == 0) || (collision.gameObject.tag == "Scales"))
        {
            myRB.mass = MyMass;
            if(myScales != null)
            {
                myScales.GetComponentInParent<Scales>().removeMass(myMass, myScales);
                myScales = null;
                scalesChanged = true;
            }
            
        }

        
    }

    public void OnStartDrag()
    {
        myRB.useGravity = false;
        closestDropPoint = GetClosestDropPoint();
        dragStartPos2D = new Vector2(transform.position.x, transform.position.z);
        dragStartPosY = transform.position.y;

    }

    public void OnEndDrag()
    {
        myRB.useGravity = true;
        myRB.velocity = Vector3.zero;
        closestDropPoint = null;
        dragStartPos2D = Vector2.zero;
        dragStartPosY = 0;
    }

    public void AfterDragPosChange()
    {
        gameObject.TryGetComponent<Rigidbody>(out Rigidbody myRB);
        //check if closest point has changed
        GameObject tempClosestPoint = GetClosestDropPoint();
        if(tempClosestPoint != closestDropPoint)
        {
            closestDropPoint = tempClosestPoint;
            //dragStartPos2D = new Vector2(transform.position.x, transform.position.z);
            //dragStartPosY = transform.
        }
        Vector2 dropPoint2DPos = new Vector2(closestDropPoint.transform.position.x, closestDropPoint.transform.position.z);
        Vector2 myCurrent2DPos = new Vector2(transform.position.x, transform.position.z);

        // y = mx+c
        // c = y-intersect = the start y pos of the weighable object
        // m = gradient (delta y / delta x) between the (dropPoint y - start weighable y) / distance between droppoint2D and start weighable2D
        // x = the current distance between the now and the startpos
        // y can then be calculated.

        float c = dragStartPosY;
        float m = (closestDropPoint.transform.position.y - dragStartPosY) / (Vector2.Distance(dragStartPos2D, dropPoint2DPos));
        float x = Vector2.Distance(myCurrent2DPos, dragStartPos2D);
        float y = (m * x) + c;

        //turn distance into velocity to be added to the already calculated x and z velocity in the DragAndDrop manager
        float velocityY = (y - transform.position.y) * Time.deltaTime * 40;
        myRB.velocity = new Vector3(myRB.velocity.x, velocityY, myRB.velocity.z);

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
