using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {

    public Vector2 xyPos;
    public EditorShipModule childModule = null;

	// Use this for initialization
	void Start () {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
	}

    public bool HasItem()
    {
        if(childModule != null)
        {
            return true;
        }
        return false;
    }

    public void OnDrop(PointerEventData eventData)//Handles when a module gets dropped on a slot.
    {
        EditorShipModule droppedItem = eventData.pointerDrag.GetComponent<EditorShipModule>();

        if (droppedItem == null) {//Case when a new instance from a module bank is dropped
            droppedItem = eventData.pointerDrag.GetComponent<ModuleBank>().dragInstance.GetComponent<EditorShipModule>();
        }
        else if (droppedItem.dragInstance != null)//Case when a new instance from a module middle-clicked is dropped
        {
            droppedItem = droppedItem.dragInstance.GetComponent<EditorShipModule>();
        }

        if (childModule != null && childModule != droppedItem)
        {
            Destroy(childModule.gameObject);
        }

        if (droppedItem.currentSlot != null)
        {
            droppedItem.currentSlot.childModule = null;
        }

        droppedItem.transform.position = transform.position;
        droppedItem.transform.SetParent(transform);
        droppedItem.currentSlot = this;
        childModule = droppedItem;

    }

}
