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

    public void SetColor(Color color)
    {
        GetComponent<Image>().color = color;
    }

    public void OnDrop(PointerEventData eventData)//Handles when a module gets dropped on a slot.
    {
        SetColor(Color.white);

        EditorShipModule droppedItem = eventData.pointerDrag.GetComponent<EditorShipModule>();

        if (droppedItem == null) {//Case when a new instance from a module bank is dropped
            DropNew(eventData);
        }
        else if (droppedItem.dragInstance != null)//Case when a new instance from a module middle-clicked is dropped
        {
            DropDuplcated(droppedItem);
        }
        else
        {
            DropItem(droppedItem);
        }

        Editor.instance.DisplayStats();
    }

    void DropItem(EditorShipModule droppedItem)
    {
        RemoveFromPreviousSlot(droppedItem);
        DestroyItemInThisSlot(droppedItem);
        droppedItem.currentSlot = this;
        childModule = droppedItem;
    }

    void DropNew(PointerEventData eventData)
    {
        EditorShipModule droppedItem = eventData.pointerDrag.GetComponent<ModuleBank>().dragInstance.GetComponent<EditorShipModule>();
        DropItem(droppedItem);
    }

    void DropDuplcated(EditorShipModule droppedItem)
    {
        droppedItem = droppedItem.dragInstance;
        DropItem(droppedItem);
        droppedItem.ModuleToParentSlot();
    }

    void RemoveFromPreviousSlot(EditorShipModule droppedItem)
    {
        if (droppedItem.currentSlot != null)
        {
            droppedItem.currentSlot.childModule = null;
        }
    }

    void DestroyItemInThisSlot(EditorShipModule droppedItem)
    {
        if (childModule != null && childModule != droppedItem)
        {
            Destroy(childModule.gameObject);
        }
    }

}
