using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Bucket : MonoBehaviour
{
    [SerializeField] TextMeshPro massText;
    [SerializeField] private Transform myForcePoint;
    [SerializeField] private Rigidbody myRB;
    [SerializeField] private HingeJoint myHinge;

    [SerializeField] private float totalMass = 0;

    Dictionary<int, float> myWeighables = new Dictionary<int, float>();

    JointSpring mySpring = new JointSpring();

    // Start is called before the first frame update
    void Start()
    {
        mySpring = myHinge.spring;

    }
    
    public void addMass(GameObject weighable, float mass)
    {
        //get the unioque ID of the weighable game object and use it as the key
        //this prevents double counting of mass as was sometimes seen when onTriggerExit / Enter failed to work / was janky
        myWeighables[weighable.GetInstanceID()] = mass;
    }

    public void removeMass(GameObject weighable)
    {
        myWeighables.Remove(weighable.GetInstanceID());
    }

    
    void FixedUpdate()
    {
        //using FixedUpdate for Physics calcs
        totalMass = myWeighables.Values.Sum();

        massText.text = totalMass.ToString() + " Kg";
        //Vector3 myForce = Vector3.up * totalMass * Physics.gravity.y * Time.deltaTime;
        Vector3 myForce = Vector3.up * totalMass * Physics.gravity.y;

       
        //DEBUG ONLY
        myHinge.spring = mySpring;
        //END DEBUG
        


        myRB.AddForceAtPosition(myForce, myForcePoint.position);
    }
}
