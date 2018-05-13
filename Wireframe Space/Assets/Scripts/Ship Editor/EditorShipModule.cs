using UnityEngine;
using UnityEngine.EventSystems;

//Represents the editor pieces that are used to build a ship
public class EditorShipModule : MonoBehaviour, IBeginDragHandler,
IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public Slot currentSlot = null;
    public int id;
    public bool editable = true;

    public EditorShipModule dragInstance;

    public void OnBeginDrag(PointerEventData eventData)//These functions handle all of the dragging and dropping of the modules when builting the editor.
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            DragModule();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            DuplcateModule(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            DeleteModule();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!editable) return;
            transform.position = eventData.position;
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            dragInstance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ModuleToParentSlot();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            dragInstance.ModuleToParentSlot();
            dragInstance = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!editable) return;
            transform.SetParent(transform.parent.parent.parent);
            transform.position = eventData.position;
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            DeleteModule();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ModuleToParentSlot();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            DeleteModule();
        }
    }

    public void ModuleToParentSlot()
    {
        if (!editable) return;
        if (currentSlot == null) Destroy(gameObject);
        Slot parentSlot = Editor.instance.GetSlot(currentSlot.xyPos);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(parentSlot.transform);
        transform.position = parentSlot.transform.position;
    }

    void DragModule()
    {
        if (!editable) return;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        currentSlot.SetColor(Color.white);
    }

    void DuplcateModule(PointerEventData eventData)
    {
        dragInstance = Instantiate(GameManager.instance.database.GetEditorModule(id));
        dragInstance.transform.position = eventData.position;
        dragInstance.transform.SetParent(Editor.instance.transform);
        dragInstance.transform.localScale = new Vector3(1, 1, 1);
        dragInstance.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void DeleteModule()
    {
        if (currentSlot != null)
        {
            currentSlot.childModule = null;
        }
        currentSlot.SetColor(Color.white);
        Editor.instance.DisplayStats();
        Destroy(gameObject);
    }

}