using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CheckBounds : MonoBehaviour
{

    [SerializeField] Bounds myBounds;
    [SerializeField] Vector3 myBoundsSize;

    [SerializeField] List<Bounds> childBounds;
    [SerializeField] List<Vector3> childBoundsSize;

    public Vector3 MyBoundsSize { get => myBoundsSize;  }

    // Start is called before the first frame update
    void Awake()
    {
        calcBoundsNow();
    }

    void Start()
    {
        calcBoundsNow();
    }

    public void calcBoundsNow()
    {
        //myBounds = GetComponent<Renderer>().bounds;


        myBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            myBounds.Encapsulate(r.bounds);
        }
        myBoundsSize = myBounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
