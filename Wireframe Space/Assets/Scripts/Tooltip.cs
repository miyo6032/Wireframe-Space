using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Class used to join the node that activated it with the actual tooltip
public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public bool mouseOverTooltip = false;
    public Text Title;
    public Text Body;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverTooltip = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOverTooltip = false;
        MapMenu.instance.DeactivateTooltip();
    }

}
