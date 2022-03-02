using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scales : MonoBehaviour
{
    //[SerializeField] private float totalMass1 = 0;
    //[SerializeField] private float totalMass2 = 0;
    [SerializeField] public Transform leftSpawnPoint;
    [SerializeField] public Transform rightSpawnPoint;
    //[SerializeField] private Transform myForcePoint1;
    //[SerializeField] private Transform myForcePoint2;
    //[SerializeField] private Collider myCollider1;
    //[SerializeField] private Collider myCollider2;

    //[SerializeField] Rigidbody balanceBoardRigidBody;

    [SerializeField] private Bucket leftBucket;
    [SerializeField] private Bucket rightBucket;


    public void Awake()
    {
        //myRb = GetComponent<Rigidbody>();
        //Debug.Assert(myForcePoint1 != null, "CentraliseWeight:Awake myForcePoint1 cannot be null");
        //Debug.Assert(myForcePoint2 != null, "CentraliseWeight:Awake myForcePoint2 cannot be null");
        //Debug.Assert(myCollider1 != null, "CentraliseWeight:Awake myCollider1 cannot be null");
        //Debug.Assert(myCollider2 != null, "CentraliseWeight:Awake myCollider2 cannot be null");
        //Debug.Assert(balanceBoardRigidBody != null, "CentraliseWeight:Awake balanceBoardRigidBody cannot be null");
    }

    public void resetScaleWeights()
    {
        leftBucket.ResetScales();
        rightBucket.ResetScales();
    }
    //Old code
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag != "WeighableObject") { return; }

    //    ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
    //    int points = collision.GetContacts(contactPoints);

    //    for (int i = 0; i < points; i++)
    //    {
    //        if (contactPoints[i].thisCollider == myCollider1)
    //        {
    //            collision.gameObject.GetComponent<WeighableObject>().MyScales = myCollider1;
    //        }
    //        else if (contactPoints[i].thisCollider == myCollider2)
    //        {
    //            collision.gameObject.GetComponent<WeighableObject>().MyScales = myCollider2;
    //        }
    //    }
    //}

    //Old code
    //public void addMass(float mass, Collider collider)
    //{
    //    if(collider == myCollider1)
    //    {
    //        totalMass1 += mass;
    //    }
    //    else if(collider == myCollider2)
    //    {
    //        totalMass2 += mass;
    //    }
    //}

    //Old code
    //public void removeMass(float mass, Collider collider)
    //{
    //    if (collider == myCollider1)
    //    {
    //        totalMass1 -= mass;
    //    }
    //    else if (collider == myCollider2)
    //    {
    //        totalMass2 -= mass;
    //    }
    //}

    //Old code
    //private void FixedUpdate()
    //{
    //    Vector3 myForce1 = Vector3.up * totalMass1 * Physics.gravity.y * Time.deltaTime;
    //    Vector3 myForce2 = Vector3.up * totalMass2 * Physics.gravity.y * Time.deltaTime;
        

    //    balanceBoardRigidBody.AddForceAtPosition(myForce1, myForcePoint1.position);
    //    balanceBoardRigidBody.AddForceAtPosition(myForce2, myForcePoint2.position);
        
    //}
}
