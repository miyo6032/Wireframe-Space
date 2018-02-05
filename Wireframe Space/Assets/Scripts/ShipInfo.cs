using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The individual ship profiles that appear in the saved ship list and their gui workings
public class ShipInfo : MonoBehaviour {

    public Text infoText;

    public Text title;

    public GameObject shipVisual;

    public int id;

    //Two different methods for the map ship list and the editor ship list
    public void ActivateDeletePanel()
    {
        Editor.instance.savedShipList.ActivateDeletePanel(this);
    }

    public void ActivateDeletePanelMap()
    {
        MapMenu.instance.shipList.ActivateDeletePanel(this);
    }

    public void LoadShip()
    {
        Editor.instance.LoadShip(id, false);
    }

    public void LoadPreset()
    {
        Editor.instance.LoadShip(id, true);
    }

    public void SetAsCurrentShip(bool preset)
    {
        MapMenu.instance.SetCurrentShip(id, preset);
        MapMenu.instance.shipList.ClearList();
        MapMenu.instance.OpenMapMenu();
        MapMenu.instance.shipList.gameObject.SetActive(false);
    }

}
