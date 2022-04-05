//Author: Craig Zeki
//Student ID: zek21003166


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragable : MonoBehaviour
{
    //private bool selected = false;
    public bool locked = false;

    private void OnEnable()
    {
        TouchInputManager.Instance.OnStartTouch += TouchStarted;
        TouchInputManager.Instance.OnEndTouch += TouchEnded;
    }

    private void OnDisable()
    {
        TouchInputManager.Instance.OnStartTouch -= TouchStarted;
        TouchInputManager.Instance.OnEndTouch -= TouchEnded;
    }


    public void TouchStarted(Vector2 screenPosition, float time)
    {
        Debug.Log("BOO!");
        //selected = false;
        if (!locked)
        {

        }
    }

    public void TouchEnded(Vector2 screenPosition, float time)
    {
        Debug.Log("BOO!");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
