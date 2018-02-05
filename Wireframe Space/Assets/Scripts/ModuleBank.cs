using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Implements functionality of the parts bank in the ship editor
public class ModuleBank : MonoBehaviour,
IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public EditorShipModule modulePrefab;

    public GameObject dragInstance;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragInstance = Instantiate(modulePrefab.gameObject);
        dragInstance.transform.position = eventData.position;
        dragInstance.transform.SetParent(Editor.instance.transform);
        dragInstance.transform.localScale = new Vector3(1, 1, 1);
        dragInstance.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragInstance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)//Handles when the module is dropped from the bank
    {
        Slot parentSlot = dragInstance.GetComponent<EditorShipModule>().currentSlot;
        if (parentSlot != null)
        {
            dragInstance.transform.SetParent(parentSlot.transform);
            dragInstance.transform.position = parentSlot.transform.position;
            dragInstance.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            Destroy(dragInstance.gameObject);
        }
        Editor.instance.DisplayStats();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Editor.instance.tooltipTitle.text = modulePrefab.title;
        Editor.instance.tooltipBody.text = "Health: " + modulePrefab.representativeModule.health +
            "\nCost: " + modulePrefab.cost +
            "\nMass: " + modulePrefab.representativeModule.mass + 
            "\n" + modulePrefab.desc;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Editor.instance.tooltipTitle.text = "";
        Editor.instance.tooltipBody.text = "";
    }
}
