using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iDragAndDrop
{
    void OnStartDrag();
    void OnEndDrag();

    void AfterDragPosChange();
}
