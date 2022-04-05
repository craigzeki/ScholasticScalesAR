//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Bucket : MonoBehaviour
{
    //Debug items
    [SerializeField] TextMeshPro massText;
    JointSpring mySpring = new JointSpring();
    //End Debug items


    //References to the key points of the scales
    [SerializeField] private Transform myForcePoint;
    [SerializeField] private Rigidbody myRB;
    [SerializeField] private HingeJoint myHinge;

    //keep track of the total weight in this bucket
    [SerializeField] private float totalMass = 0;

    //keep track of each of the objects in the bucket
    Dictionary<int, float> myWeighables = new Dictionary<int, float>();

    void Start()
    {
        //Debug
        mySpring = myHinge.spring;
        //End debug
    }
    public void ResetScales()
    {
        //clear the list of objects and set the total weight to 0Kg
        myWeighables.Clear();
        totalMass = 0;
    }
    public void addMass(GameObject weighable, float mass)
    {
        //get the unioque ID of the weighable game object and use it as the key
        //this prevents double counting of mass as was sometimes seen when onTriggerExit / Enter failed to work / was janky
        //Update of total mass will occur in FixedUpdate
        myWeighables[weighable.GetInstanceID()] = mass;
    }

    public void removeMass(GameObject weighable)
    {
        //remove the weighable object from the list - mass will be removed on FixedUpdate
        myWeighables.Remove(weighable.GetInstanceID());
    }

    
    void FixedUpdate()
    {
        //using FixedUpdate for Physics calcs

        //add up all the weights
        totalMass = myWeighables.Values.Sum();

        //Debug Text
        massText.text = totalMass.ToString() + " Kg";
        //Vector3 myForce = Vector3.up * totalMass * Physics.gravity.y * Time.deltaTime;
        
        //Do not need Time.deltatime as force is being constantly applied
        Vector3 myForce = Vector3.up * totalMass * Physics.gravity.y;

       
        //DEBUG ONLY
        myHinge.spring = mySpring;
        //END DEBUG
        
        //Add the force to the balance arm
        myRB.AddForceAtPosition(myForce, myForcePoint.position);
    }
}
