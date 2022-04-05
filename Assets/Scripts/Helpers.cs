//Author: Craig Zeki
//Student ID: zek21003166

namespace ZekstersLab.Helpers
{
    using UnityEngine;

    public static class Helpers
    {
        public static void matchSizes(GameObject objectTarget, GameObject objectToChange)
        {
            //guards
            if (objectTarget.GetComponent<CheckBounds>() == null) { return; }
            if (objectToChange.GetComponent<CheckBounds>() == null) { return; }

            Vector3 targetSize = objectTarget.GetComponent<CheckBounds>().MyBoundsSize;
            Vector3 startingScaledSize = objectToChange.GetComponent<CheckBounds>().MyBoundsSize;
            //Vector3 startingUnscaledSize = Vector3.Scale(startingScaledSize, objectToChange.transform.lossyScale);
            Vector3 startingUnscaledSize = new Vector3(
                                            startingScaledSize.x / objectToChange.transform.lossyScale.x,
                                            startingScaledSize.y / objectToChange.transform.lossyScale.y,
                                            startingScaledSize.z / objectToChange.transform.lossyScale.z
                                            );

            //objectToChange.transform.localScale = Vector3.Scale(targetSize, startingUnscaledSize);
            objectToChange.transform.localScale = new Vector3(
                                            targetSize.x / startingUnscaledSize.x,
                                            targetSize.y / startingUnscaledSize.y,
                                            targetSize.z / startingUnscaledSize.z
                                            );
        }

        public static void matchXSizeLockedAspectRatio(GameObject objectTarget, GameObject objectToChange)
        {
            //guards
            if (objectTarget.GetComponent<CheckBounds>() == null) { return; }
            if (objectToChange.GetComponent<CheckBounds>() == null) { return; }

            float targetSize = objectTarget.GetComponent<CheckBounds>().MyBoundsSize.x;
            float startingScale = objectToChange.transform.lossyScale.x;
            float startingScaledSize = objectToChange.GetComponent<CheckBounds>().MyBoundsSize.x;
            float startingUnscaledSize = startingScaledSize / objectToChange.transform.lossyScale.x;
            float newScale = targetSize / startingUnscaledSize;

            float ratio = newScale / startingScale;

            objectToChange.transform.localScale = ratio * objectToChange.transform.localScale;
        }

        public static void matchXSizeLockedAspectRatio(GameObject objectTarget, GameObject objectToChange, float factor)
        {
            matchXSizeLockedAspectRatio(objectTarget, objectToChange);

            objectToChange.transform.localScale *= factor;
        }

    }
}
