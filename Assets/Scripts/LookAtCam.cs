//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LookAtCam : MonoBehaviour
{
    private TextMeshPro myText;
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<TextMeshPro>(out myText);
    }

    // Update is called once per frame
    void Update()
    {
        if(myText != null)
        {
            myText.rectTransform.LookAt(Camera.main.transform);
            myText.rectTransform.Rotate(Vector3.up, 180);
        }
        else
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(Vector3.up, 180);

        }
        
    }
}
