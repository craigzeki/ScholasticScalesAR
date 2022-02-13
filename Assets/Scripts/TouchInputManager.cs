using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1)] //run before the other scripts
public class TouchInputManager : MonoBehaviour
{
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;
    public delegate void TouchPerformedEvent(Vector2 position);
    public event TouchPerformedEvent OnTouchPerformed;

    private TouchControls touchControls;

    private static TouchInputManager instance;

    public static TouchInputManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<TouchInputManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        touchControls = new TouchControls();
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    private void Start()
    {
        touchControls.Touch.TouchInput.started += ctx => StartTouch(ctx);
        touchControls.Touch.TouchInput.canceled += ctx => EndTouch(ctx);
        touchControls.Touch.TouchPosition.performed += ctx => TouchDrag(ctx);
    }

    private void TouchDrag(InputAction.CallbackContext context)
    {
        if (OnTouchPerformed != null) OnTouchPerformed(touchControls.Touch.TouchPosition.ReadValue<Vector2>());
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null) OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);

    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null) OnEndTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
    }

    public static bool IsTouchOverUI(Vector2 pos)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = pos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        return (results.Count > 0);

        
    }

    
}
