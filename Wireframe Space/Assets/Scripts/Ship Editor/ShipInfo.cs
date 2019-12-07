using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The individual ship profiles that appear in the saved ship list and their gui workings
public class ShipInfo : MonoBehaviour {

    public Text infoText;

    public Text title;

    public GameObject shipVisual;

    private ShipIndex shipIndex;

    private SavedShipList shipList;

    //Two different methods for the map ship list and the editor ship list
    public void ActivateDeletePanel()
    {
        shipList.ActivateDeletePanel(this);
    }

    public void LoadShip()
    {
        Editor.instance.LoadShip(shipIndex);
    }

    public void Init(string infoText, string title, ShipIndex shipIndex, ShipSave ship)
    {
        this.infoText.text = infoText;
        this.title.text = title;
        this.shipIndex = shipIndex;

        foreach (ModuleSaveData module in ship.modules)
        {
            EditorShipModule mod = Instantiate(GameManager.instance.database.GetEditorModule(module.Id));
            Editor.instance.SetHexPositon(new Vector2(module.xPos, module.yPos), shipVisual.transform.position, mod.gameObject, Editor.instance.unitSize * Editor.instance.shipInfoUnitScale);
            mod.transform.localScale = Editor.instance.shipInfoUnitScale * new Vector3(1, 1, 1);
            mod.transform.SetParent(transform);
            mod.editable = false;
        }
    }

    public void SetAsCurrentShip()
    {
        MapMenu.instance.SetCurrentShip(shipIndex);
        MapMenu.instance.shipList.ClearList();
        MapMenu.instance.OpenMapMenu();
        MapMenu.instance.shipList.gameObject.SetActive(false);
    }

}
