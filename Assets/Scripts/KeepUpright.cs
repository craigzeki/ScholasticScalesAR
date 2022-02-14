using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class KeepUpright : MonoBehaviour
{ 
    [SerializeField] TextMeshPro debugText;
    // Start is called before the first frame update

    private void Awake()
    {

        //myARTrackedImageManager.enabled = false;
    }

    void Start()
    {
        lookAtCamera();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));
        //debugText.text = "Rotation: " + transform.rotation.eulerAngles.ToString();
    }

    public void lookAtCamera()
    {
        //transform.LookAt(Camera.main.transform);
        //transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));
        //debugText.text = "Rotation: " + transform.rotation.eulerAngles.ToString();

        var cameraForward = Camera.main.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
        transform.rotation = Quaternion.LookRotation(cameraBearing);
        //debugText.text = "Rotation: " + transform.rotation.eulerAngles.ToString();
    }
    
}
