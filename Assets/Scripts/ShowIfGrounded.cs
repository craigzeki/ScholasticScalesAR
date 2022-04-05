//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIfGrounded : MonoBehaviour
{
    private WeighableObject myParentWeighable;
    private Renderer myRenderer;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert((myParentWeighable = GetComponentInParent<WeighableObject>()) != null, "ShowIfGrounded:Awake: myParentWeighable cannot be null");
        Debug.Assert((myRenderer = GetComponent<Renderer>()) != null, "ShowIfGrounded:Awake: myRenderer cannot be null");
    }

    // Update is called once per frame
    void Update()
    {
        if(myParentWeighable.IsGrounded)
        {
            myRenderer.enabled = true;
        }
        else
        {
            myRenderer.enabled = false;
        }
    }
}
