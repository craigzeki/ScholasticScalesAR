using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject temp = other.gameObject;

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject temp = other.gameObject;
    }
}
