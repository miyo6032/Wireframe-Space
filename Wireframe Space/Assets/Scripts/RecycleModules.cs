using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Destroys editorShipModules when they are dragged back into the bank
public class RecycleModules : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        EditorShipModule droppedItem = eventData.pointerDrag.GetComponent<EditorShipModule>();

        if (droppedItem == null)//No need to handle an item from a bank - it will get destroyed automatically
        {
            return;
        }
        else if (droppedItem.dragInstance != null)//Case when a new instance from a module middle-clicked is dropped
        {
            droppedItem = droppedItem.dragInstance.GetComponent<EditorShipModule>();
        }

        if (droppedItem.currentSlot != null)
        {
            droppedItem.currentSlot.childModule = null;
        }

        Destroy(droppedItem.gameObject);
        Editor.instance.DisplayStats();
    }

}
