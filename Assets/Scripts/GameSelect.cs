using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelect : MonoBehaviour
{
    [SerializeField] private GameTypes gameType;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if((other.tag == "Bucket") && !triggered)
        {
            triggered = true;
            StartCoroutine(waitforonesec());

        }
    }

    IEnumerator waitforonesec()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.GameType = gameType;
    }
}
