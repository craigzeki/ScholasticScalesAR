//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iDragAndDrop
{
    void OnStartDrag();
    void OnEndDrag();

    void AfterDragPosChange();
}
