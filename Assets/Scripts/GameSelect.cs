using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelect : MonoBehaviour
{
    [SerializeField] private GameTypes gameType;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        //if not already on a bucket and now we are
        if((other.tag == "Bucket") && !triggered)
        {
            //set triggered and the run the ChangeGameType coroutine
            triggered = true;
            StartCoroutine(ChangeGameType());

        }
    }

    IEnumerator ChangeGameType()
    {
        //Wait one second so that the user can see the scales tilt after the block is placed
        yield return new WaitForSeconds(1);
        GameManager.Instance.GameType = gameType;
    }
}
