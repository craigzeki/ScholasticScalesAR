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

    public void onStartDrag()
    {
        myRB.useGravity = false;
    }

    public void onEndDrag()
    {
        myRB.useGravity = true;
        myRB.velocity = Vector3.zero;
    }
}
