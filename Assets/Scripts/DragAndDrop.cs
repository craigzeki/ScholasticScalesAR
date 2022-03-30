using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] ARRaycastManager arRaycastManager;
    [SerializeField] float touchSpeed = 10f;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool touchInProgress = false;
    private Vector2 touchPos = Vector2.zero;
    private float touchStartTime = 0f;
    private float touchDuration = 0f;



    private void Awake()
    {
        Debug.Assert(arRaycastManager != null, "DragAndDrop:Awake: arRaycastManager cannot be null");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    private void TouchStarted(Vector2 screenPosition, float time)
    {
        touchInProgress = true;
        touchPos = screenPosition;
        touchStartTime = time;
        touchDuration = 0f;

        Ray myRay = Camera.main.ScreenPointToRay(Touchscreen.current.position.ReadValue());
        RaycastHit myHit;
        if (Physics.Raycast(myRay, out myHit))
        {
            if (myHit.collider != null)
            {
                //if raycast has hit an object, test if it is dragable or contains a dragable object as a child
                GameObject go = null;
                if(myHit.collider.gameObject.GetComponent<iDragAndDrop>() != null)
                {
                    go = myHit.collider.gameObject;
                }
                else if(myHit.collider.gameObject.GetComponentInParent<iDragAndDrop>() != null)
                {
                    go = myHit.collider.gameObject.transform.parent.gameObject;
                }

                if (go != null) { StartCoroutine(DragUpdate(go)); }
            }
        }

    }

    private void TouchEnded(Vector2 screenPosition, float time)
    {
        touchInProgress = false;
        touchPos = screenPosition;
        touchDuration = time - touchStartTime;
    }

    private IEnumerator DragUpdate(GameObject gameObject)
    {
        gameObject.TryGetComponent<Rigidbody>(out Rigidbody myRB);
        gameObject.TryGetComponent<iDragAndDrop>(out var iDragComponent);
        iDragComponent?.OnStartDrag();
        //get the initial distance from the screen, we will use this later to enforce the distance does not change while dragging
        float initialDistanceFromScreen = Vector3.Distance(gameObject.transform.position, Camera.main.transform.position);
        //float initialDistanceFromScreen = Vector3.Distance(gameObject.transform.position, Camera.main.nearClipPlane);

        //while the mouse button is down
        while (touchInProgress)
        {
            //get the current point the mouse is at
            Ray myRay = Camera.main.ScreenPointToRay(Touchscreen.current.position.ReadValue());
            //calculate the vector between the mouse and the object
            Vector3 direction = myRay.GetPoint(initialDistanceFromScreen) - gameObject.transform.position;
            direction = new Vector3(direction.x, 0, direction.z);
            myRB.velocity = direction * touchSpeed;
            iDragComponent?.AfterDragPosChange();
            yield return new WaitForFixedUpdate();
        }
        iDragComponent?.OnEndDrag();
    }

}
