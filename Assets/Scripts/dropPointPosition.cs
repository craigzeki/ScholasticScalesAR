using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropPointPosition : MonoBehaviour
{
    [SerializeField] Transform bucket1Transform;
    [SerializeField] Transform bucket2Transform;

    float yOffset;

    private void Awake()
    {
        yOffset = transform.position.y - getHighPoint();
    }

    public float getHighPoint()
    {
        return Mathf.Max(bucket1Transform.position.y, bucket2Transform.position.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, getHighPoint() + yOffset, transform.position.z);
    }
}
