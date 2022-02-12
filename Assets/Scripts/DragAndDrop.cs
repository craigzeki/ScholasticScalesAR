using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private InputAction mouseClicked;

    [SerializeField] private float mouseSpeed = 10f;
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
        mouseClicked.Enable();
        mouseClicked.performed += MousePressed;
      
    }

    private void OnDisable()
    {
        mouseClicked.performed -= MousePressed;
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        Ray myRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit myHit;
        if (Physics.Raycast(myRay, out myHit))
        {
            if ((myHit.collider != null) && (myHit.collider.gameObject.CompareTag("WeighableObject") || myHit.collider.gameObject.GetComponent<iDragAndDrop>() != null))
            {
                StartCoroutine(DragUpdate(myHit.collider.gameObject));
            }
        }
    }

    private void Touch(InputAction.CallbackContext context)
    {
        Ray myRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit myHit;
        if (Physics.Raycast(myRay, out myHit))
        {
            if ((myHit.collider != null) && (myHit.collider.gameObject.CompareTag("WeighableObject") || myHit.collider.gameObject.GetComponent<iDragAndDrop>() != null))
            {
                StartCoroutine(DragUpdate(myHit.collider.gameObject));
            }
        }
    }

    private IEnumerator DragUpdate(GameObject gameObject)
    {
        gameObject.TryGetComponent<Rigidbody>(out Rigidbody myRB);
        gameObject.TryGetComponent<iDragAndDrop>(out var iDragComponent);
        iDragComponent?.onStartDrag();
        //get the initial distance from the screen, we will use this later to enforce the distance does not change while dragging
        float initialDistanceFromScreen = Vector3.Distance(gameObject.transform.position, Camera.main.transform.position);

        //while the mouse button is down
        while (mouseClicked.ReadValue<float>() != 0)
        {
            //get the current point the mouse is at
            Ray myRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            //calculate the vector between the mouse and the object
            Vector3 direction = myRay.GetPoint(initialDistanceFromScreen) - gameObject.transform.position;
            myRB.velocity = direction * mouseSpeed;
            yield return new WaitForFixedUpdate();
        }
        iDragComponent?.onEndDrag();
    }
}
