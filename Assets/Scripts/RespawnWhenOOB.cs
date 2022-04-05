using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeighableObject))]
public class RespawnWhenOOB : MonoBehaviour
{
    [SerializeField] private float oOBOffset = -15.0f;
    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {


        if (transform.position.y - startPos.y < oOBOffset)
        {
            transform.position = startPos;
        }

    }

}
