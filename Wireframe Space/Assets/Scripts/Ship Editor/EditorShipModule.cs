using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Represents the editor pieces that are used to build a ship
public class EditorShipModule : MonoBehaviour, IBeginDragHandler,
IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public Slot currentSlot = null;
    public int id;
    public bool editable = true;

    public GameObject dragInstance;

    public void OnBeginDrag(PointerEventData eventData)//These functions handle all of the dragging and dropping of the modules when builting the editor.
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!editable) return;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            currentSlot.GetComponent<Image>().color = Color.white;
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            dragInstance = Instantiate(GameManager.instance.database.GetEditorModule(id).gameObject);
            dragInstance.transform.position = eventData.position;
            dragInstance.transform.SetParent(Editor.instance.transform);
            dragInstance.transform.localScale = new Vector3(1, 1, 1);
            dragInstance.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentSlot != null)
            {
                currentSlot.childModule = null;
            }

            Editor.instance.DisplayStats();
            Destroy(gameObject);
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
            if (!editable) return;
            Slot parentSlot;
            if ((parentSlot = Editor.instance.GetSlot(currentSlot.xyPos)) != null)
            {
                transform.SetParent(parentSlot.transform);
                transform.position = parentSlot.transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            Editor.instance.DisplayStats();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Slot parentSlot = dragInstance.GetComponent<EditorShipModule>().currentSlot;
            if (parentSlot != null)
            {
                dragInstance.transform.SetParent(parentSlot.transform);
                dragInstance.transform.position = parentSlot.transform.position;
                dragInstance.GetComponent<CanvasGroup>().blocksRaycasts = true;
                dragInstance = null;
            }
            else
            {
                Destroy(dragInstance.gameObject);
            }
            Editor.instance.DisplayStats();
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
            if (currentSlot != null)
            {
                currentSlot.childModule = null;
            }

            Editor.instance.DisplayStats();
            Destroy(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!editable) return;
            Slot parentSlot;
            if ((parentSlot = Editor.instance.GetSlot(currentSlot.xyPos)) != null)
            {
                transform.SetParent(parentSlot.transform);
                transform.position = parentSlot.transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            if (currentSlot != null)
            {
                currentSlot.childModule = null;
            }

            Editor.instance.DisplayStats();
            Destroy(gameObject);
        }
        if (Input.GetMouseButton(2))
        {

        }
    }
}