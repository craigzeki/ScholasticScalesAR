//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckBounds))]
public class cheveron : MonoBehaviour
{
    public GameObject transmittingObject;
    public GameObject targetObject;

    [SerializeField] float fallSpeed = 1;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //SetMyScale();
    }

    
    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector3(transmittingObject.transform.position.x,
                                        transform.position.y - (fallSpeed * Time.deltaTime),
                                        transmittingObject.transform.position.z);
        if(transform.position.y < targetObject.transform.position.y)
        {
            Destroy(this.gameObject);
        }
    }
}
