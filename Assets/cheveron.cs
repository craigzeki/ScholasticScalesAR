using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheveron : MonoBehaviour
{
    public GameObject transmittingObject;
    public GameObject targetObject;

    [SerializeField] float fallSpeed = 1;
    [SerializeField] float stopDistanceFromTarget = 0.0f;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMyScale();
        Debug.Log("");
    }

    public void SetMyScale()
    {
        transform.localScale = new Vector3(transmittingObject.transform.lossyScale.x, transmittingObject.transform.lossyScale.y, transmittingObject.transform.lossyScale.z / 10);
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
