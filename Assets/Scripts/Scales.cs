//Author: Craig Zeki
//Student ID: zek21003166

using UnityEngine;

public class Scales : MonoBehaviour
{
    //Reference to the two spawn points for the weighable object collectiosn
    [SerializeField] public Transform leftSpawnPoint;
    [SerializeField] public Transform rightSpawnPoint;

    //Reference to the two buckets on the balance arm
    [SerializeField] private Bucket leftBucket;
    [SerializeField] private Bucket rightBucket;

    public void resetScaleWeights()
    {
        //reset the scales
        leftBucket.ResetScales();
        rightBucket.ResetScales();
    }

}
