//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FixRotationToZero : MonoBehaviour
{
    private TextMeshPro myText;
    private Rigidbody myRB;
    //public bool enabled;
    // Start is called before the first frame update

    private void Awake()
    {
        //enabled = false;
    }

    void Start()
    {
        TryGetComponent<TextMeshPro>(out myText);
        TryGetComponent<Rigidbody>(out myRB);
    }

    // Update is called once per frame
    void Update()
    {
        //guard
        if (!enabled) { return; }

        if (myText != null)
        {


            myText.rectTransform.Rotate(Vector3.zero);
        }
        else if(myRB != null)
        {
            myRB.MoveRotation(Quaternion.Euler(Vector3.Scale(Quaternion.Euler(0, 1, 0).eulerAngles, myRB.rotation.eulerAngles)));
        }
        else
        {
            
            transform.Rotate(Vector3.zero);

        }

    }
}
