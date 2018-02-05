using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShiftButton : MonoBehaviour, IPointerClickHandler
{

    public Vector2 xyPos;

    public void OnPointerClick(PointerEventData eventData)
    {
        Editor.instance.ShiftModules(xyPos);
    }
}
