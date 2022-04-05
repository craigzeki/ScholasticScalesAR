//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanLeftRight : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private int screenWidth;
    [SerializeField] private int screenBuffer = 10;
    private Vector2 screenEdgeTarget;
    float valueToLerp;
    Vector2 velocity = Vector2.zero;
    private Image myImage;
    
    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Screen.width;
        //set the target to be the right edge
        screenEdgeTarget.x = screenWidth / 2;
        screenEdgeTarget.y = 0;


        myImage = GetComponent<Image>();
        //set my position to be the left edge
        myImage.rectTransform.anchoredPosition = screenEdgeTarget * -1;
    }

    // Update is called once per frame
    void Update()
    {
        //smoothdamp to the target position (similar to lerp but with damping)
        myImage.rectTransform.anchoredPosition = Vector2.SmoothDamp(myImage.rectTransform.anchoredPosition, screenEdgeTarget, ref velocity, smoothTime);
        if(Mathf.Abs(myImage.rectTransform.anchoredPosition.x) > (Mathf.Abs(screenEdgeTarget.x) - screenBuffer))
        {
            //if we reached the current targeted edge (within a small range), flip
            screenEdgeTarget *= -1;
        }
    }

}
